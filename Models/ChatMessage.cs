using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class ChatMessage
    {
        public enum MessageTypeEnum
        {
            Text,
            Image
        }
        public int Id { get; set; }
        public string MessageOrImageKey { get; set; }
        public MessageTypeEnum Type { get; set; }
    }
}
