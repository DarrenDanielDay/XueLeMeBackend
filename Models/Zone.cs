using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public ZoneDetail ToDetail()
        {
            return new ZoneDetail { Id = Id, ZoneName = Name };
        }
    }
}
