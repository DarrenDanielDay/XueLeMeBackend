using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public NotificationDetail ToDetail()
        {
            return new NotificationDetail
            {
                NotificationType = NotificationType,
                Content = Content,
                Id = Id
            };
        }
    }
    public enum NotificationTypeEnum
    {
        ChatMessage,
        JoinRequest,
        NewMemberJoined,
        UserQuitted,
        UserKicked,
        Replied,
        Mentioned,
    }
}
