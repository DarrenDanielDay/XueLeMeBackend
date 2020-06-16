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
            Authentications.Include(a => a.User);
            ChatGroups.Include(g => g.Creator).Include(g => g.Memberships).Include(g => g.JoinGroupRequests);
            JoinGroupRequests.Include(r => r.User).Include(r => r.Group);
            GroupMemberships.Include(m => m.ChatGroup).Include(m => m.User);

        }
        // Models
        public DbSet<AdditionalImage> AdditionalImages { get; set; }
        public DbSet<AppliedTag> AppliedTags { get; set; }
        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<BinaryFile> BinaryFiles { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatRecord> ChatRecords { get; set; }
        public DbSet<CheckRecord> CheckRecords { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<ScheduleItem> ScheduleItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TextAndImageContent> TextAndImageContents { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Zone> Zones { get; set; }
        // Fragments
        public DbSet<ResetPasswordRequest> ResetPasswordRequests { get; set; }
        public DbSet<MailRegisterRequest> MailRegisterRequests { get; set; }
        public DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }
        public DbInitializer Initializer { get; }
    }
}
