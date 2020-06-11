using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class AppliedTag
    {
        public int Id { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
        public string PostTagDisplayName { get; set; }
    }
}
