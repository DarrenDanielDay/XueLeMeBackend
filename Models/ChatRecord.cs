using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class ChatRecord
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public User Sender { get; set; }
        public ChatGroup Group { get; set; }
        public ChatMessage Message { get; set; }
    }
}
