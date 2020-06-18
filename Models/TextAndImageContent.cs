using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;
namespace XueLeMeBackend.Models
{
    public class TextAndImageContent
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual ICollection<AdditionalImage> Images { get; set; }
        public TextAndImageContentDetail ToDetail()
        {
            return new TextAndImageContentDetail
            {
                Id = Id,
                Text = Text,
                Images = Images.Select(i => i.ImageFileMD5).ToList()
            };
        }
    }
}
