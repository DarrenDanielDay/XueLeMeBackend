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

    public class GroupBrief
    {
        public int Id { get; set; }
        public string Name { get; set; }
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

    public class JoinGroupRequestDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }

    public class TextAndImageContentDetail
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<string> Images { get; set; }
    }

    public class AnonymousDetail
    {
        public int UserId { get; set; }
        public string Fakename { get; set; }
    }

    public class TopicDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public AnonymousDetail PublisherDetail { get; set; }
        public int ZoneId { get; set; }
        public TextAndImageContentDetail ContentDetail { get; set; }
        public ICollection<string> Tags { get; set; }
    }

    public class ReplyDetail
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public AnonymousDetail User { get; set; }
        public TextAndImageContentDetail ContentDetail { get; set; }
    }

    public class ZoneDetail
    {
        public int Id { get; set; }
        public string ZoneName { get; set; }
    }
}
