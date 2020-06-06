using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class GroupMembership
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatGroupId { get; set; }
        public User User { get; set; }
        public ChatGroup ChatGroup { get; set; }

    }
}
