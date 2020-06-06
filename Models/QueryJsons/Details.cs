using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.QueryJsons
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
    }

    public class GroupDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserDetail Owner { get; set; }
        public IEnumerable<UserDetail> Members { get; set; }
    }

    public class ChatRecordDetail
    {
        public UserDetail Sender { get; set; }
        public GroupDetail Group { get; set; }
        public DateTime CreatedTime { get; set; }
        public ChatMessage.MessageTypeEnum MessageType { get; set; }
        public string Content { get; set; }
    }
}
