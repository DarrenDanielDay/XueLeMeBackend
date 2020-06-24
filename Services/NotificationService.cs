using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Hubs;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;
namespace XueLeMeBackend.Services
{
    public class NotificationService
    {
        public NotificationService(
            IConnectionService connectionService,            
            XueLeMeContext context,
            IHubContext<NotificationHub> hubContext
        )
        {
            ConnectionService = connectionService;
            Context = context;
            HubContext = hubContext;
        }

        public IConnectionService ConnectionService { get; }
        public XueLeMeContext Context { get; }
        public IHubContext<NotificationHub> HubContext { get; }

        public Task<IEnumerable<Notification>> GetNotifications(int userId)
        {
            return Task.FromResult(Context.Notifications.Where(n => n.UserId == userId).ToList().AsEnumerable());
        }

        public async Task<ServiceResult<object>> ReadNotification(int id)
        {
            var notification = Context.Notifications.FirstOrDefault(n => n.Id == id);
            if (notification == null)
            {
                return NotFound("该消息不存在");
            }
            else
            {
                Context.Notifications.Remove(notification);
                await Context.SaveChangesAsync();
                return Success("已成功阅读消息");
            }
        }

        public async Task<ServiceResult<object>> ReadNotifications(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var notification = Context.Notifications.FirstOrDefault(n => n.Id == id);
                if (notification != null)
                {
                    Context.Notifications.Remove(notification);
                }
            }
            await Context.SaveChangesAsync();
            return Success("已阅读所有可能的消息");
        }

        public async Task<ServiceResult<object>> Notify(int userId, string message, NotificationTypeEnum notificationType)
        {
            var user = Context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("用户不存在");
            }
            if (ConnectionService.IsOnline(userId))
            {
                await HubContext.Clients.Client(ConnectionService.GetConnectionId(userId)).SendAsync("OnNotify",(int)notificationType, message);
                return Success("在线通知成功");
            } 
            else
            {
                var notification = new Notification
                {
                    Content = message,
                    UserId = userId,
                    NotificationType = notificationType
                };
                Context.Notifications.Add(notification);
                await Context.SaveChangesAsync();
                return Success("离线通知保存成功");
            }
        }

        public async Task<ServiceResult<object>> NotifyGroupMembers(int groupId, string message, NotificationTypeEnum notificationType)
        {
            var group = Context.ChatGroups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound("群聊不存在");
            }
            var groupMemberIds = Context.GroupMemberships.Where(m => m.ChatGroupId == groupId).Select(m => m.UserId).ToList();
            if (notificationType == NotificationTypeEnum.ChatMessage)
            foreach(var userId in groupMemberIds)
            {
                if (!ConnectionService.IsOnline(userId))
                {
                    var notification = new Notification
                    {
                        Content = message,
                        UserId = userId,
                        NotificationType = notificationType
                    };
                    Context.Notifications.Add(notification);
                }
            }
            await Context.SaveChangesAsync();
            await HubContext.Clients.Group(groupId.ToString()).SendAsync("OnNotify", notificationType, message);
            return Success("群聊通知发送成功");
        }
    }
}
