using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;

namespace XueLeMeBackend.Hubs
{
    public class ChatHub: Hub
    {
        public ChatHub(ILogger<ChatHub> logger, XueLeMeContext xueLeMeContext, IGroupService groupService, IAccountService accountService)
        {
            Logger = logger;
            Logger.LogInformation("new ChatHub!");
            XueLeMeContext = xueLeMeContext;
            GroupService = groupService;
            AccountService = accountService;
        }

        public ILogger<ChatHub> Logger { get; }
        public XueLeMeContext XueLeMeContext { get; }
        public IGroupService GroupService { get; }
        public IAccountService AccountService { get; }  
        public static Dictionary<int, string> UserIdToConnectionId = new Dictionary<int, string>();
        public static Dictionary<string, int> ConnectionIdToUserId = new Dictionary<string, int>();
        public async Task<string> SendMessage(int userid, int groupid, int type, string content)
        {
            var group = await GroupService.FromGroupId(groupid);
            if (group.State != ServiceResultEnum.Exist)
            {
                return group.Detail;
            }
            var user = await AccountService.UserFromId(userid);
            if (user.State != ServiceResultEnum.Exist)
            {
                return user.Detail;
            }
            var userInGroup = await GroupService.HasMemberShip(user.ExtraData, group.ExtraData);
            if (!userInGroup.ExtraData)
            {
                return userInGroup.Detail;
            }
            var targets = Clients.Group(groupid.ToString());
            var message = new ChatMessage
            {
                MessageOrImageKey = content,
                Type = (ChatMessage.MessageTypeEnum)type
            };
            ChatRecord chatRecord = new ChatRecord
            {
                Sender = user.ExtraData,
                CreatedTime = DateTime.Now,
                Group = group.ExtraData,
                Message = message
            };
            XueLeMeContext.ChatMessages.Add(message);
            XueLeMeContext.ChatRecords.Add(chatRecord);
            await XueLeMeContext.SaveChangesAsync();
            await targets.SendAsync("OnReceiveMessage", userid, groupid, type, content, chatRecord.CreatedTime);
            return "发送成功";
        }

        public async Task<string> JoinRoom(int userid)
        {
            var user = await AccountService.UserFromId(userid);
            if (user.State != ServiceResultEnum.Exist)
            {
                Logger.LogInformation($"user id {userid} does not exsit");
                return "用户不存在";
            }
            var groups = (await GroupService.MyJoinedGroups(user.ExtraData)).ExtraData.ToList();
            await QuitCurrentRoomAsync();
            groups.ForEach(async g =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, g.Id.ToString());
            });
            
            ConnectionIdToUserId[Context.ConnectionId] = userid;
            UserIdToConnectionId[userid] = Context.ConnectionId;
            return "加入聊天室成功";
        }

        private async Task QuitCurrentRoomAsync()
        {
            var connid = Context.ConnectionId;
            if (ConnectionIdToUserId.ContainsKey(connid))
            {
                var uid = ConnectionIdToUserId[connid];
                ConnectionIdToUserId.Remove(connid);
                UserIdToConnectionId.Remove(uid);
                var groups = (await GroupService.MyJoinedGroups(new User { Id=uid})).ExtraData.ToList();
                groups.ForEach(async g => await Groups.RemoveFromGroupAsync(connid, g.Id.ToString()));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Logger.LogInformation($"{Context.ConnectionId} 's connection is lost");
            await QuitCurrentRoomAsync();
        }

        public override async Task OnConnectedAsync()
        {
            Logger.LogInformation($"{Context.ConnectionId} 's connection established");
        }

    }
}
