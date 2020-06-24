using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Services;
using Microsoft.AspNetCore.SignalR;
using XueLeMeBackend.Data;

namespace XueLeMeBackend.Hubs
{
    public class NotificationHub: Hub
    {
        public NotificationHub(IConnectionService connectionService, XueLeMeContext xueLeMeContext)
        {
            ConnectionService = connectionService;
            XueLeMeContext = xueLeMeContext;
        }

        public IConnectionService ConnectionService { get; }
        public XueLeMeContext XueLeMeContext { get; }

        public async Task<string> JoinAsUser(int userId)
        {
            if (!XueLeMeContext.Users.Where(u => u.Id == userId).Any())
            {
                return "账号不存在";
            }
            var groups = XueLeMeContext.GroupMemberships.Where(m => m.UserId == userId).Select(m => m.ChatGroupId).ToList();
            ConnectionService.Attach(Context.ConnectionId, userId);
            foreach(var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.ToString());
            }
            return "成功与服务器建立连接";
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionService.Detach(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
