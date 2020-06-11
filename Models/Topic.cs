using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public User Publisher { get; set; }
        public int PublisherId { get; set; }
        public Zone Zone { get; set; }
        public int ZoneId { get; set; }
        public TextAndImageContent Content { get; set; }
        public int TopicId { get; set; }
        public ICollection<AppliedTag> AppliedTags { get; set; }
        public ICollection<Reply> Replies { get; set; }
    }
}
