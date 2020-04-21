using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class ReplyNotification
    {
        public int Id { get; set; }
        public User Receiver { get; set; }
        public PostReply Reply { get; set; }
    }
}
