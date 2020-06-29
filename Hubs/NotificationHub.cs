using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Services;
using Microsoft.AspNetCore.SignalR;
using XueLeMeBackend.Data;
using Microsoft.Extensions.Logging;
using XueLeMeBackend.Models;

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
            if (ConnectionService.IsOnline(userId))
            {
                string connectionId = ConnectionService.GetConnectionId(userId);
                if (connectionId != Context.ConnectionId)
                {
                    await Clients.Client(connectionId).SendAsync("OnNotify", (int)NotificationTypeEnum.ForceOut, "您的账号在别的设备被登录，可能是密码泄露，请重新登陆！");
                    ConnectionService.Detach(connectionId);
                }
            }
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
