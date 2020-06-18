using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class CreateTopicForm
    {
        public string Title { get; set; }
        public int UserId { get; set; }
        public int ZoneId { get; set; }
        public ICollection<string> Tags { get; set; }
        public string Content { get; set; }
        public ICollection<string> Images { get; set; }
    }
}
