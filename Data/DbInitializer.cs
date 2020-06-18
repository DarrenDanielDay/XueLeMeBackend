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
            Logger.LogInformation("Initializing database...");
            //if (!Configuration.GetValue<bool>("IsServer"))
            {
                Logger.LogWarning("Removing database...");
                context.Database.EnsureDeleted();
            }
            context.Database.EnsureCreated();
            var user1 = CreateUser(context, "614434935@qq.com", "password");
            var user2 = CreateUser(context, "jamesnm2015@163.com", "123456");
            var user3 = CreateUser(context, "1215196803@qq.com", "123456");
            var group1 = CreateGroup(context, "group1", user1);
            var group2 = CreateGroup(context, "group2", user1);
            JoinGroup(context, user2, group1);
            JoinGroup(context, user3, group2);
            CreateZone(context, "数学吧");
            context.SaveChanges();
            Logger.LogInformation("Initialization completed.");
        }

        private User CreateUser(XueLeMeContext context, string mailAdress, string password)
        {
            var user = new User
            {
                Authentications = new List<Authentication>(),
            };
            var auth = new Authentication
            {
                Type = Authentication.AuthTypeEnum.MailAddress,
                AuthToken = mailAdress,
                User = user,
                CreatedTime = DateTime.Now,
                UserToken = password
            };
            user.Authentications.Add(auth);
            context.Add(user);
            context.SaveChanges();
            return user;
        }
        private ChatGroup CreateGroup(XueLeMeContext context, string name, User owner)
        {
            ChatGroup group = new ChatGroup
            {
                Creator = owner,
                CreatorId = owner.Id,
                GroupName = name,
                Memberships = new List<GroupMembership> {  }
            };
            GroupMembership membership = new GroupMembership { User = owner, ChatGroup = group, Role = GroupRole.Owner };
            group.Memberships.Add(membership);
            context.ChatGroups.Add(group);
            context.SaveChanges();
            return group;
        }
        private void JoinGroup(XueLeMeContext context, User user, ChatGroup group, GroupRole role = GroupRole.Member)
        {
            GroupMembership membership = new GroupMembership
            {
                User = user,
                ChatGroup = group,
                Role = role,
            };
            context.GroupMemberships.Add(membership);
            context.SaveChanges();
        }

        private void CreateZone(XueLeMeContext context, string name)
        {
            Zone zone = new Zone
            {
                Name = name,
            };
            context.Zones.Add(zone);
            context.SaveChanges();
        }
    }
}
