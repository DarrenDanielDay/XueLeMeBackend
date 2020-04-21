using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public User Publisher { get; set; }
        public ICollection<PostReply> Replies { get; set; }
    }
}
