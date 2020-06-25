using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;
namespace XueLeMeBackend.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public virtual Topic Topic { get; set; }
        public int TopicId { get; set; }
        public virtual User Responder { get; set; }
        public int ResponderId { get; set; }
        public virtual Reply Reference { get; set; }
        public virtual TextAndImageContent Content { get; set; }
        public int ContentId { get; set; }
        public ReplyDetail ToDetail()
        {
            return new ReplyDetail
            {
                Id = Id,
                TopicId = TopicId,
                ContentDetail = Content.ToDetail(),
                ReferenceId = Reference?.Id,
                User = new AnonymousDetail
                {
                    FakeName = Topic.Anonymous.FirstOrDefault(a => a.UserId == ResponderId).DisplayName,
                    UserId = ResponderId,
                },
            };
        }
    }
}
