using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;
namespace XueLeMeBackend.Models
{
    public class ChatRecord
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int GroupId { get; set; }
        public virtual ChatGroup Group { get; set; }
        public int MessageId { get; set; }
        public virtual ChatMessage Message { get; set; }
        public ChatRecordDetail ToDetail()
        {
            return new ChatRecordDetail
            {
                Id = Id,
                Content = Message.MessageOrImageKey,
                CreatedTime = CreatedTime,
                Group = Group.ToDetail(),
                MessageType = Message.Type,
                Sender = Sender.ToDetail()
            };
        }
    }
}
