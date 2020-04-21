using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class PostReply
    {
        public int Id { get; set; }
        public User Responder { get; set; }
        public PostReply Reference { get; set; }
        public ReplyContent Content { get; set; }
    }
}
