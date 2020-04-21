using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
namespace XueLeMeBackend.Data
{
    public class DbInitializer
    {
        public static void Init(XueLeMeContext context)
        {
            var mail = "614434935@qq.com";
            var user = new User
            {
                Authentications = new List<Authentication>(),
            };
            var auth = new Authentication
            {
                Type = Authentication.AuthTypeEnum.MailAddress,
                User = user,
                UserToken = "password",
                AuthToken = mail,
                CreatedTime = DateTime.Now
            };
            context.Users.Add(user);
            context.Authentications.Add(auth);
            var request = new ResetPasswordRequest
            {
                EmailAddress = mail,
                CreatedTime = DateTime.Now,
                Token = "a"
            };
            context.ResetPasswordRequests.Add(request);
            context.SaveChanges();
        }
    }
}
