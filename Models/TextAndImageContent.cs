using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class TextAndImageContent
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual ICollection<AdditionalImage> Images { get; set; }
    }
}
