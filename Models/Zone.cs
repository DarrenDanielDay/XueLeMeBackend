using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
