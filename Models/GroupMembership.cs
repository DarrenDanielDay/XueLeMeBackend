using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public enum GroupRole
    {
        Owner,
        Member,
    }
    public class GroupMembership
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatGroupId { get; set; }
        public GroupRole Role { get; set; }
        public virtual User User { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }

    }
}
