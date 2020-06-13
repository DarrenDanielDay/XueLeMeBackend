using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models.Fragments
{
    public class JoinGroupRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public User User { get; set; }
        public ChatGroup Group { get; set; }
        public JoinGroupRequestDetail ToDetail()
        {
            return new JoinGroupRequestDetail
            {
                Id = Id,
                UserId = UserId,
                GroupId = GroupId,
            };
        }
    }
}
