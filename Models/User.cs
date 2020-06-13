using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public BinaryFile Avatar { get; set; }
        public ICollection<Authentication> Authentications { get; set; } = new List<Authentication>();
        public ICollection<ScheduleItem> ScheduleItems { get; set; } = new List<ScheduleItem>();
        public UserDetail ToDetail()
        {
            return new UserDetail { Id = Id, Nickname = Nickname, Avatar = Avatar?.MD5 };
        }
    }
}
