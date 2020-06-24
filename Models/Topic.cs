using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;
namespace XueLeMeBackend.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual User Publisher { get; set; }
        public int PublisherId { get; set; }
        public virtual Zone Zone { get; set; }
        public int ZoneId { get; set; }
        public virtual TextAndImageContent Content { get; set; }
        public int ContentId { get; set; }
        public virtual ICollection<AppliedTag> AppliedTags { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
        public ICollection<Anonymous> Anonymous { get; set; }
        public TopicDetail ToDetail()
        {
            return new TopicDetail
            {
                Id = Id,
                PublisherDetail = new AnonymousDetail
                {
                    FakeName = Anonymous.Where(a => a.UserId == Publisher.Id).FirstOrDefault().DisplayName,
                    UserId = Publisher.Id,
                },
                Tags = AppliedTags.Select(t => t.TagDisplayName).ToList(),
                Title = Title,
                ZoneId = ZoneId,
                ContentDetail = Content.ToDetail(),
            };
        }
    }
}
