using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class MakeReplyForm
    {
        public int UserId { get; set; }
        public int TopicId { get; set; }
        public int? ReferenceId { get; set; }
        public int MyProperty { get; set; }
        public string Content { get; set; }
        public ICollection<string> Images { get; set; }
    }
}
