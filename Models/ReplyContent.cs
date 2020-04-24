using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class ReplyContent
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<BinaryFile> Images { get; set; }
    }
}
