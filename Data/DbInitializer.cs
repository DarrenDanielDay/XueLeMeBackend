using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
using XueLeMeBackend.Services;

namespace XueLeMeBackend.Data
{
    public class DbInitializer
    {
        public DbInitializer(ILogger<DbInitializer> logger, IConfiguration configuration, MD5Service mD5Service)
        {
            Initialized = false; 
            Logger = logger;
            Configuration = configuration;
            MD5Service = mD5Service;
        }
        public bool Initialized { get; set; }
        public ILogger<DbInitializer> Logger { get; }
        public IConfiguration Configuration { get; }
        public MD5Service MD5Service { get; }

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
            if (Configuration.GetValue<bool>("IsServer"))
            {
                //return;
            }
            Logger.LogWarning("Removing database...");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var user1 = CreateUser(context, "614434935@qq.com", "password");
            var user2 = CreateUser(context, "jamesnm2015@163.com", "123456");
            var user3 = CreateUser(context, "1215196803@qq.com", "123456");
            var group1 = CreateGroup(context, "group1", user1);
            var group2 = CreateGroup(context, "group2", user1);
            JoinGroup(context, user2, group1);
            JoinGroup(context, user3, group2);
            CreateZone(context, "数学吧");
            CreateTag(context, "高等数学");
            CreateTag(context, "线性代数");
            AddNotification(context, user1.Id, "测试通知1");
            AddNotification(context, user1.Id, "测试通知2");
            AddNotification(context, user1.Id, "测试通知3");
            CreateFile(context, "Assets/DarrenDanielDay.jpg");
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

        public void CreateTag(XueLeMeContext context, string tagString)
        {
            Tag tag = new Tag { DisplayName = tagString };
            context.Tags.Add(tag);
            context.SaveChanges();
        }

        public void AddNotification(XueLeMeContext context, int userId, string content)
        {
            Notification notification = new Notification
            {
                Content = content,
                NotificationType = NotificationTypeEnum.Replied,
                UserId = userId,
            };
            context.Notifications.Add(notification);
            context.SaveChanges();
        }

        public void CreateFile(XueLeMeContext context, string filename, string contentType = "image/jpg")
        {
            using (FileStream fileStream = File.OpenRead(filename))
            {
                BinaryFile file = new BinaryFile
                {
                    Bytes=new byte[fileStream.Length],
                    FileName=filename,
                    ContentType=contentType,
                    CreatedTime=DateTime.Now,
                };
                fileStream.Read(file.Bytes, 0, Convert.ToInt32(fileStream.Length));
                file.MD5 = MD5Service.MD5Generate(file.Bytes);
                context.BinaryFiles.Add(file);
                Logger.LogInformation($"Created file with MD5: {file.MD5}");
            }
            context.SaveChanges();
        }
    }
}
