using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public BinaryFile Avatar { get; set; }
        public ICollection<Authentication> Authentications { get; set; }
        public ICollection<ScheduleItem> ScheduleItems { get; set; }
    }
}
