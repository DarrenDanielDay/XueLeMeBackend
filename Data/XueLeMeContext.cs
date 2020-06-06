using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;

namespace XueLeMeBackend.Data
{
    public class XueLeMeContext: DbContext
    {
        public XueLeMeContext(DbContextOptions<XueLeMeContext> contextOptions, DbInitializer initializer) : base(contextOptions)
        {
            Initializer = initializer;
            initializer.Init(this);
        }
        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatRecord> ChatRecords { get; set; }
        public DbSet<CheckRecord> CheckRecords { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReply> PostReplies { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<ReplyContent> ReplyContents { get; set; }
        public DbSet<ReplyNotification> ReplyNotifications { get; set; }
        public DbSet<ScheduleItem> ScheduleItems { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<ResetPasswordRequest> ResetPasswordRequests { get; set; }
        public DbSet<MailRegisterRequest> MailRegisterRequests { get; set; }
        public DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }
        public DbSet<BinaryFile> BinaryFiles { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbInitializer Initializer { get; }
    }
}
