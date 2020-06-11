using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
        public User Responder { get; set; }
        public int ResponderId { get; set; }
        public Reply Reference { get; set; }
        public TextAndImageContent Content { get; set; }
        public int ContentId { get; set; }
    }
}
