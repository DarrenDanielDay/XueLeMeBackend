using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace XueLeMeBackend.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
