using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Services;
using Microsoft.AspNetCore.SignalR;
using XueLeMeBackend.Data;
using Microsoft.Extensions.Logging;

namespace XueLeMeBackend.Hubs
{
    public class NotificationHub: Hub
    {
        public NotificationHub(IConnectionService connectionService, XueLeMeContext xueLeMeContext, ILogger<NotificationHub> logger)
        {
            ConnectionService = connectionService;
            XueLeMeContext = xueLeMeContext;
            Logger = logger;
        }

        public IConnectionService ConnectionService { get; }
        public XueLeMeContext XueLeMeContext { get; }
        public ILogger<NotificationHub> Logger { get; }

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
        public override async Task OnConnectedAsync()
        {
            Logger.LogInformation("Connected with connection id = {0}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionService.Detach(Context.ConnectionId);
            Logger.LogInformation("Disconnected with connection id = {0}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
