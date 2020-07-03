using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                return;
            }
            Logger.LogWarning("Removing database...");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var avatar1 = CreateFile(context, "Assets/DarrenDanielDay.jpg");
            var avatar2 = CreateFile(context, "Assets/tom.jpg");
            var avatar3 = CreateFile(context, "Assets/jk.jpg");
            var lhopital = CreateFile(context, "Assets/LHopital.jpg");
            var timg = CreateFile(context, "Assets/timg.jpg");
            var format = CreateFile(context, "Assets/format.jpg");
            var user1 = CreateUser(context, "614434935@qq.com", "password", "Darren", avatar1.MD5);
            var user2 = CreateUser(context, "jamesnm2015@163.com", "123456", "james", avatar2.MD5);
            var user3 = CreateUser(context, "1215196803@qq.com", "123456", "ZYH", avatar3.MD5);
            var group1 = CreateGroup(context, "group1", user1);
            var group2 = CreateGroup(context, "group2", user1);
            JoinGroup(context, user2, group1);
            JoinGroup(context, user3, group2);
            var mathBar = CreateZone(context, "数学吧");
            CreateZone(context, "英语吧");
            var advancedMath = CreateTag(context, "高等数学");
            CreateTag(context, "线性代数");
            var topic1 = CreateTopic(context, mathBar, "大家都怎么用洛必达法则？", "洛必达镇楼", user1, "An8kf01X",new List<string> { timg.MD5 }, new List<string> {advancedMath.DisplayName });
            var reply2 = CreateReply(context, topic1, user3, "7IH10Mzq", "等价无穷小做不来的时候用", new List<string> { }, null);
            CreateReply(context, topic1, user3, null, "用等价无穷小他不香吗", new List<string> { format.MD5 }, reply2);
            var reply1 = CreateReply(context, topic1, user2, "x09oUYLm", "管他呢，洛就完了\n洛必达法则天下第一！", new List<string> { lhopital.MD5 }, null);
            CreateReply(context, topic1, user1, null, "hhh确实\n洛必达法则，永远滴神", new List<string> { }, reply1);
            context.SaveChanges();
            Logger.LogInformation("Initialization completed.");
        }

        private User CreateUser(XueLeMeContext context, string mailAdress, string password,string userName = null, string avatar = null)
        {
            var user = new User
            {
                Authentications = new List<Authentication>(),
                Avatar = context.BinaryFiles.FirstOrDefault(f => f.MD5 == avatar),
                Nickname = userName,
            };
            var auth = new Authentication
            {
                Type = Authentication.AuthTypeEnum.MailAddress,
                AuthToken = mailAdress,
                User = user,
                CreatedTime = DateTime.Now,
                UserToken = password,
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

        private Zone CreateZone(XueLeMeContext context, string name)
        {
            Zone zone = new Zone
            {
                Name = name,
            };
            context.Zones.Add(zone);
            context.SaveChanges();
            return zone;
        }

        public Tag CreateTag(XueLeMeContext context, string tagString)
        {
            Tag tag = new Tag { DisplayName = tagString };
            context.Tags.Add(tag);
            context.SaveChanges();
            return tag;
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

        public BinaryFile CreateFile(XueLeMeContext context, string filename, string contentType = "image/jpg")
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
                context.SaveChanges();
                return file;
            }
        }

        public TextAndImageContent CreateContent(XueLeMeContext context, string text, IEnumerable<string> images)
        {
            var content = new TextAndImageContent
            {
                Text = text,
            };
            context.TextAndImageContents.Add(content);
            context.SaveChanges();
            foreach (var image in images)
            {
                var apply = new AdditionalImage
                {
                    ImageFileMD5 = image,
                    TextAndImageContentId = content.Id,
                };
                context.AdditionalImages.Add(apply);
            }
            context.SaveChanges();
            return content;
        }

        public Topic CreateTopic(XueLeMeContext context,Zone zone, string title, string text, User publisher, string fakename, IEnumerable<string> images, IEnumerable<string> tags)
        {
            var content = CreateContent(context, text, images);
            var topic = new Topic
            {
                Content = content,
                Title = title,
                Publisher = publisher,
                PublisherId = publisher.Id,
                ContentId = content.Id,
                Zone = zone,
                Anonymous = new List<Anonymous>() { },
            };

            topic.Anonymous.Add(new Anonymous { Topic = topic, User = publisher, DisplayName = fakename, });
            context.Topics.Add(topic);
            context.SaveChanges();
            foreach (var tag in tags)
            {
                var existingTag = context.Tags.FirstOrDefault(t => t.DisplayName == tag);
                if (existingTag == null)
                {
                    existingTag = new Tag
                    {
                        DisplayName = tag,
                    };
                    context.Tags.Add(existingTag);
                    context.SaveChanges();
                }
                context.AppliedTags.Add(new AppliedTag
                {
                    Tag = existingTag,
                    Topic = topic,
                });
            }
            context.SaveChanges();
            return topic;
        }
        public Reply CreateReply(XueLeMeContext context, Topic topic, User replier, string fakename, string text, IEnumerable<string> images, Reply reference)
        {
            var content = CreateContent(context, text, images);
            Reply reply = new Reply {
                 Content=content,
                  Reference =reference,
                   Responder =replier,
                    Topic = topic,
                    
            };
            topic =  context.Topics.Include(t => t.Anonymous).Where(t => t.Id == topic.Id).FirstOrDefault();
            if (!topic.Anonymous.Where(a => a.UserId == replier.Id).Any())
            {
                topic.Anonymous.Add(new Anonymous { Topic = topic, User = replier, DisplayName = fakename });
            }
            context.Replies.Add(reply);
            context.SaveChanges();
            return reply;
        }
    }
}
