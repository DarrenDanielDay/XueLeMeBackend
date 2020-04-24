using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class QQMailService :MailAddressVerifier, IMailService
    {
        public static readonly string Username = "darren_daniel_day";
        public static readonly string SmtpHost = "qq.com";
        public static readonly string Password = "wfttinftvjxwebbc";
        public readonly MailAddress SenderAddress = new MailAddress("darren_daniel_day@qq.com");
        public readonly NetworkCredential Credential = new NetworkCredential(Username, Password);
        public readonly SmtpClient Smtp = new SmtpClient("smtp.qq.com", 25);

        public async Task<ServiceResult<bool>> SendMail(string to, string title, string content)
        {
            var valid = await IsValidMailAddress(to);
            if (valid.State != ServiceResultEnum.Valid)
            {
                return Invalid(false, valid.Detail);
            }
            MailMessage message = new MailMessage(from: SenderAddress, to: new MailAddress(to));
            message.Subject = title;
            message.Body = content;
            message.IsBodyHtml = true;
            Smtp.Credentials = Credential;
            return await Task.Run(() => {
                try
                {
                    Smtp.Send(message);
                }
                catch (Exception e)
                {
                    return Fail(false,"邮件发送失败"+ e.Message);
                }
                return Success(true, "邮件发送成功");
            });
        }
    }
}
