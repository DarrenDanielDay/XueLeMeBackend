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
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
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

            var mail2 = "jamesnm2015@163.com";
            var user2 = new User
            {
                Authentications = new List<Authentication>()
            };
            var auth2 = new Authentication
            {
                Type = Authentication.AuthTypeEnum.MailAddress,
                User = user2,
                UserToken = "123456",
                AuthToken = mail2,
                CreatedTime = DateTime.Now
            };
            user.Authentications.Add(auth2);
            context.Users.Add(user2);
            context.SaveChanges();
        }
    }
}
