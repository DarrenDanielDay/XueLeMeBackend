using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.Fragments;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models
{
    public class ChatGroup
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public string GroupName { get; set; }
        public ICollection<GroupMembership> Memberships { get; set; }
        public ICollection<JoinGroupRequest> JoinGroupRequests { get; set; }
        public GroupDetail ToDetail()
        {
            var memberships = Memberships.ToList();
            var members = new List<UserDetail>();
            memberships.ForEach(m => members.Add(m.User.ToDetail()));
            return new GroupDetail
            {
                Id=Id,
                Name=GroupName,
                Owner=Creator.ToDetail(),
                Members=members
            };
        }
    }
}
