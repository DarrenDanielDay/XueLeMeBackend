using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Anonymous
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
    }
}
