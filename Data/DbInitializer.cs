using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
namespace XueLeMeBackend.Data
{
    public class DbInitializer
    {
        public DbInitializer(ILogger<DbInitializer> logger, IConfiguration configuration)
        {
            Initialized = false; 
            Logger = logger;
            Configuration = configuration;
        }
        public bool Initialized { get; set; }
        public ILogger<DbInitializer> Logger { get; }
        public IConfiguration Configuration { get; }

        public void Init(XueLeMeContext context) 
        {
            if (!Initialized) {
                Initialized = true;
                DoInit(context);
            }
        }
        public void DoInit(XueLeMeContext context)
        {
            if (!Configuration.GetValue<bool>("IsServer")) {
                context.Database.EnsureDeleted();
            }
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
            user.Authentications.Add(auth);
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
            user2.Authentications.Add(auth2);
            context.Users.Add(user2);
            context.SaveChanges();
        }
    }
}
