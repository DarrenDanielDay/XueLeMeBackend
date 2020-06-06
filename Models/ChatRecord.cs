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
        public User Sender { get; set; }
        public ChatGroup Group { get; set; }
        public ChatMessage Message { get; set; }
        public ChatRecordDetail ToDetail()
        {
            return new ChatRecordDetail
            {
                Content = Message.MessageOrImageKey,
                CreatedTime = CreatedTime,
                Group = Group.ToDetail(),
                MessageType = Message.Type,
                Sender = Sender.ToDetail()
            };
        }
    }
}
