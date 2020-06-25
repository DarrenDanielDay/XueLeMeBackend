using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class SendMessageForm
    {
        public int UserId { get; set; }
        public int ChatGroupId { get; set; }
        public string MessageContent { get; set; }
        public ChatMessage.MessageTypeEnum MessageType { get; set; }
    }
}
