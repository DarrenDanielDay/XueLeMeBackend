using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class AdditionalImage
    {
        public int Id { get; set; }
        public virtual BinaryFile ImageFile { get; set; }
        public string ImageFileMD5 { get; set; }
        public virtual TextAndImageContent TextAndImageContent { get; set; }
        public int TextAndImageContentId { get; set; }
    }
}
