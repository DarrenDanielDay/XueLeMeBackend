using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class MailAccountService : IMailAccountService
    {
        public static readonly string AuthMailLinkFormat = "http://{0}/api/Account/MailAuth/Confirm/{1}";
        public static readonly string ResetPasswordLinkFormat = "http://{0}/api/Account/MailAuth/ResetPassword/{1}";

        public static readonly string AuthMailTitle = "激活账号";
        public static readonly string AuthMailFormat = "<h1>学了么 - 激活账号</h1><h2>您可以点击以下链接激活您的账号。该链接30分钟内有效。如非本人操作请忽略。</h2><a href={0}>{0}</a>";

        public static readonly string ResetPasswordTitle = "重置您的密码";
        public static readonly string ResetPasswordMailFormat = "<h1>学了么 - 重置密码</h1><h2>您可以点击以下链接重置您的密码。该链接1小时内有效。如非本人操作请忽略。</h2><a href={0}>{0}</a>";

        public MailAccountService(XueLeMeContext context, IMailService mailService, ISecurityService securityService, IConfiguration configuration)
        {
            Context = context;
            MailService = mailService;
            SecurityService = securityService;
            Configuration = configuration;
        }

        public XueLeMeContext Context { get; }
        public IMailService MailService { get; }
        public ISecurityService SecurityService { get; }
        public IConfiguration Configuration { get; }

        public async Task<ServiceResult<bool>> AuthMailAddress(string token)
        {
            var request = await Context.MailRegisterRequests.FirstOrDefaultAsync(r => r.RandomToken == token);
            if (request == null)
            {
                return NotFound(false,"邮箱不存在");
            }
            if (DateTime.Now.Subtract(request.CreatedTime).TotalMinutes > 30)
            {
                Context.MailRegisterRequests.Remove(request);
                await Context.SaveChangesAsync();
                return TimedOut(false,"认证超时");
            }
            var account = new User { Authentications = new List<Authentication>() };
            var auth = new Authentication
            {
                AuthToken = request.EmailAddress,
                CreatedTime = DateTime.Now,
                Type = Authentication.AuthTypeEnum.MailAddress,
                UserToken = request.Token,
                User = account
            };
            account.Authentications.Add(auth);
            Context.Users.Add(account);
            Context.MailRegisterRequests.Remove(request);
            await Context.SaveChangesAsync();
            return Success(true,"验证成功");
        }

        public async Task<ServiceResult<object>> ChangePasswordByMailAndOldPassword(string mail, string oldpassword, string newpassword)
        {
            var exist = await MailExists(mail);
            if (exist.State != ServiceResultEnum.Exist)
            {
                return Result(exist.State, exist.Detail);
            }
            if (exist.ExtraData.UserToken != oldpassword)
            {
                return Invalid("旧密码错误");
            }
            exist.ExtraData.UserToken = newpassword;
            Context.Authentications.Update(exist.ExtraData);
            await Context.SaveChangesAsync();
            return Success("修改成功");
        }

        public async Task<ServiceResult<Authentication>> MailExists(string mail)
        {
            var valid = await MailService.IsValidMailAddress(mail);
            if (!valid.ExtraData)
            {
                return Invalid<Authentication>(null);
            }
            var auth = await Context.Authentications.FirstOrDefaultAsync(at => at.Type == Authentication.AuthTypeEnum.MailAddress && at.AuthToken == mail);
            if (auth == null)
            {
                return NotFound<Authentication>(null);
            }
            return Exist(auth,"邮箱已注册");
        }

        public async Task<ServiceResult<object>> RegisterByMailAndPassword(string mail, string password)
        {
            var exist = await MailExists(mail);
            if (exist.State != ServiceResultEnum.NotFound)
            {
                return Result(exist.State, exist.Detail);
            }
            var request = await Context.MailRegisterRequests.FirstOrDefaultAsync(r => r.EmailAddress == mail);
            if (request != null)
            {
                Context.MailRegisterRequests.Remove(request);
            }
            string randomtoken = SecurityService.RandomLongString();
            Context.MailRegisterRequests.Add(new MailRegisterRequest { EmailAddress = mail, CreatedTime = DateTime.Now, RandomToken = randomtoken, Token = password });
            await Context.SaveChangesAsync();
            string url = string.Format(AuthMailLinkFormat, Configuration.GetValue<string>("Host"), randomtoken);
            var send = await MailService.SendMail(mail, AuthMailTitle, string.Format(AuthMailFormat, url));
            return send.ExtraData ? Success("注册成功，请注意查收邮件") : Fail(send.Detail);
        }

        public async Task<ServiceResult<object>> RequireResetPasswordByMail(string mail)
        {
            var exist = await MailExists(mail);
            if (exist.State != ServiceResultEnum.Exist)
            {
                return Result(exist.State,exist.Detail);
            }
            string randomtoken = SecurityService.RandomLongString();
            Context.ResetPasswordRequests.Add(new ResetPasswordRequest { CreatedTime = DateTime.Now, EmailAddress = mail, Token = randomtoken });
            await Context.SaveChangesAsync();
            string url = string.Format(ResetPasswordLinkFormat, Configuration.GetValue<string>("Host"), randomtoken);
            var result = await MailService.SendMail(mail, ResetPasswordTitle, string.Format(ResetPasswordMailFormat, url));
            return result.ExtraData ? Success("请求成功") : Fail(result.Detail);
        }

        public async Task<ServiceResult<ResetPasswordRequest>> ResetPasswordRequestExist(string token)
        {
            var request = await Context.ResetPasswordRequests.FirstOrDefaultAsync(r => r.Token == token);
            if (request == null)
            {
                return NotFound(request,"重置密码请求不存在");
            }
            if (DateTime.Now.Subtract(request.CreatedTime).TotalHours > 1)
            {
                return TimedOut(request,"重置链接超时");
            }
            return Exist(request);
        }

        public async Task<ServiceResult<object>> ResetPasswordWithResetTokenAndPassword(string token, string newpassword)
        {
            var request = await ResetPasswordRequestExist(token);
            if (request.State != ServiceResultEnum.Exist)
            {
                return Result(request.State, request.Detail);
            }
            var exist = await MailExists(request.ExtraData.EmailAddress);
            if (exist.State != ServiceResultEnum.Exist)
            {
                return Result(exist.State);
            }
            exist.ExtraData.UserToken = newpassword;
            Context.ResetPasswordRequests.Remove(request.ExtraData);
            Context.Authentications.Update(exist.ExtraData);
            await Context.SaveChangesAsync();
            return Success("重置密码成功");
        }

        public async Task<ServiceResult<bool>> VerifyMailPassword(string mail, string token)
        {
            var exist = await MailExists(mail);
            if (exist.State != ServiceResultEnum.Exist)
            {
                return NotFound(false,exist.Detail);
            }
            if (exist.ExtraData.UserToken == token)
            {   
                return Valid(true,"密码正确");
            }
            return Invalid(false,"密码错误");
        }
    }
}
