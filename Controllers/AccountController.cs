using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;
using XueLeMeBackend.Models.QueryJsons;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public AccountController(MailAccountService mailAccountService, AccountService accountService)
        {
            MailAccountService = mailAccountService;
            AccountService = accountService;
        }

        public MailAccountService MailAccountService { get; }
        public AccountService AccountService { get; }

        [HttpPost]
        [Route("MailAuth/Register")]
        public async Task<ServiceResult<object>> Register([FromBody] MailRegisterForm mailRegisterForm)
        {
            var result = await MailAccountService.RegisterByMailAndPassword(mailRegisterForm.MailAddress, mailRegisterForm.Password);
            if (result.State == ServiceResultEnum.Success)
            {
                return Success(result.Detail);
            }
            else
            {
                return Result(result.State, result.Detail);
            }
        }

        [HttpGet]
        [Route("MailAuth/Confirm/{token}")]
        public async Task<ViewResult> ConfirmRegister(string token)
        {
            var result = await MailAccountService.AuthMailAddress(token);
            return View(nameof(ServiceMessage), result);
        }

        [HttpPost]
        [Route("MailAuth/Login")]
        public async Task<ServiceResult<bool>> Login([FromBody] MailLoginForm mailLoginForm)
        {
            var result = await MailAccountService.VerifyMailPassword(mailLoginForm.MailAddress, mailLoginForm.Password);
            return result;
        }

        [HttpGet]
        [Route("MailAuth/ResetPassword/{token}")]
        public async Task<ViewResult> ResetPassword(string token)
        {
            var result = await MailAccountService.ResetPasswordRequestExist(token);
            if (result.ExtraData == null)
            {
                return View(ServiceResultViewName, ServiceMessage.NotFound("您要查看的页面不存在"));
            }
            if (result.State == ServiceResultEnum.TimedOut)
            {
                return View(ServiceResultViewName, TimedOut("该链接已失效"));
            }
            return View(new ResetPasswordForm { EmailAddress = result.ExtraData.EmailAddress });
        }

        [HttpPost]
        [Route("MailAuth/ForgetPassword")]
        public async Task<ServiceResult<object>> ForgetPassword([FromBody] ForgetPasswordForm form)
        {
            return await MailAccountService.RequireResetPasswordByMail(form.MailAddress);
        }

        [HttpPost]
        [Route("MailAuth/ResetPassword/{token}")]
        public async Task<ViewResult> ResetPassword([FromForm] ResetPasswordForm form)
        {
            if (!MyForm.ReflectCheck(form))
            {
                return View(ServiceResultViewName, Invalid("参数非法"));
            }
            if (form.Password != form.ConfirmPassword)
            {
                return View(form);
            }
            var result = await MailAccountService.ResetPasswordWithResetTokenAndPassword(form.Token, form.Password);
            if (result.State != ServiceResultEnum.Success)
            {
                return View(ServiceResultViewName, Fail(result.Detail));
            }
            return View(ServiceResultViewName, Success());
        }

        [HttpGet]
        [Route("MailAuth/QueryId")]
        public async Task<ServiceResult<int?>> UserIdOfMail(string mail)
        {
            return await MailAccountService.UserIdOfMail(mail);
        }

        [HttpGet]
        [Route("Detail/{userid}")]
        public async Task<ServiceResult<UserDetail>> Detail(int userid)
        {
            var user = await AccountService.UserFromId(userid);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<UserDetail>(user.State, null, user.Detail);
            }
            return Exist(new UserDetail { Avatar = user.ExtraData.Avatar?.MD5, Id = user.ExtraData.Id, Nickname = user.ExtraData.Nickname });
        }

        [HttpPost]
        [Route("ChangeNickname")]
        public async Task<ServiceResult<object>> ChangeNickname([FromBody] ChangeNicknameForm changeNicknameForm)
        {
            return await AccountService.ChangeNickname(changeNicknameForm.UserId, changeNicknameForm.Nickname);
        }

        [HttpPost]
        [Route("ChangeAvatar")]
        public async Task<ServiceResult<object>> ChangeAvatar([FromBody] ChangeAvatarForm changeAvatarForm)
        {
            var c = AccountService.Context;
            var user = c.Users.FirstOrDefault(u => u.Id == changeAvatarForm.UserId);
            if (user == null)
            {
                return NotFound<object>("用户不存在");
            }
            var file = c.BinaryFiles.FirstOrDefault(f => f.MD5 == changeAvatarForm.Avatar);
            if (file == null)
            {
                return NotFound<object>("文件不存在");
            }
            user.Avatar = file;
            c.Users.Update(user);
            await c.SaveChangesAsync();
            return Success("修改头像成功");
        }
    }
}