using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class ChatGroup
    {
        public int Id { get; set; }
        public User Creator { get; set; }
        public string GroupName { get; set; }
    }
}
