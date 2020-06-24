using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
using static XueLeMeBackend.Services.ServiceMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.SignalR;
using XueLeMeBackend.Hubs;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Services
{
    public class GroupService
    {
        public GroupService(XueLeMeContext context, NotificationService notificationService, IHubContext<NotificationHub> hubContext, IConnectionService connectionService)
        {
            Context = context;
            NotificationService = notificationService;
            HubContext = hubContext;
            ConnectionService = connectionService;
        }

        public XueLeMeContext Context { get; }
        public NotificationService NotificationService { get; }
        public IHubContext<NotificationHub> HubContext { get; }
        public IConnectionService ConnectionService { get; }

        public async Task<ServiceResult<object>> AgreeJoin(User owner, JoinGroupRequest request)
        {
            var owns = await OwnsGroup(owner, request.Group);
            if (!owns.ExtraData)
            {
                return Unauthorized(owns.Detail);
            }
            if (request == null)
            {
                return NotFound("该加群申请不存在");
            }
            Context.JoinGroupRequests.Remove(request);
            Context.GroupMemberships.Add(new GroupMembership { UserId = request.User.Id, ChatGroupId = request.Group.Id, Role = GroupRole.Member });
            if (ConnectionService.IsOnline(request.UserId))
            {
                await HubContext.Groups.AddToGroupAsync(ConnectionService.GetConnectionId(request.UserId), request.GroupId.ToString());
            }
            await NotificationService.NotifyGroupMembers(request.GroupId, $"{request?.User?.Nickname}加入了群聊", NotificationTypeEnum.NewMemberJoined);
            await Context.SaveChangesAsync();
            return Success("通过加群成功");
        }

        public async Task<ServiceResult<object>> ChangeName(ChatGroup chatGroup, string newName)
        {
            chatGroup.GroupName = newName;
            Context.ChatGroups.Update(chatGroup);
            await Context.SaveChangesAsync();
            return Success("修改成功");
        }

        public Task<ServiceResult<ChatGroup>> GroupFromId(int id)
        {
            var group = Context.ChatGroups.Include(g => g.Creator).FirstOrDefault(g => g.Id == id);
            if (group != null)
            {
                group.Memberships = Context.GroupMemberships.Include(m => m.User).Include(m => m.ChatGroup).Where(m => m.ChatGroupId == id).ToList();
            }
            return Task.FromResult(group == null ? NotFound<ChatGroup>(null, "群聊不存在") : Exist(group));
        }

        public Task<ServiceResult<bool>> HasMemberShip(User user, ChatGroup chatGroup)
        {
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m.ChatGroupId == chatGroup.Id);
            return Task.FromResult(membership != null ? Exist(true) : NotFound(false, "该用户不在该群"));
        }

        public async Task<ServiceResult<object>> JoinGroup(User user, ChatGroup chatGroup)
        {
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m.ChatGroupId == chatGroup.Id);
            if (membership != null)
            {
                return Exist("您已在该群中");
            }
            var request = Context.JoinGroupRequests.FirstOrDefault(r => r.User.Id == user.Id && r.Group.Id == chatGroup.Id);
            if (request != null)
            {
                return Exist("您已经申请加群");
            }
            Context.JoinGroupRequests.Add(new JoinGroupRequest { User = user, Group = chatGroup });
            await Context.SaveChangesAsync();
            await NotificationService.Notify(chatGroup.Creator.Id, $"{user?.Nickname} 申请加入群 {chatGroup.GroupName}", NotificationTypeEnum.JoinRequest);
            return Success("加群申请成功");
        }

        public async Task<ServiceResult<IEnumerable<JoinGroupRequest>>> JoinGroupRequests(int id)
        {
            var group = await GroupFromId(id);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result<IEnumerable<JoinGroupRequest>>(group.State, null, group.Detail);
            }
            var requests = Context.JoinGroupRequests.Include(r => r.User).Include(r => r.Group).Where(r => r.Group.Id == group.ExtraData.Id);
            return Exist(requests.AsEnumerable(), "查询成功");
        }

        public Task<ServiceResult<JoinGroupRequest>> JoinRequestFromId(int id)
        {
            var request = Context.JoinGroupRequests.Include(r => r.User).Include(r => r.Group).FirstOrDefault(r => r.Id == id);
            var result = request == null ? NotFound(request, "加群申请不存在") : Exist(request);
            return Task.FromResult(result);
        }

        public async Task<ServiceResult<object>> KickUser(User owner, ChatGroup chatGroup, User user)
        {
            var owns = await OwnsGroup(owner, chatGroup);
            if (!owns.ExtraData)
            {
                return Unauthorized(owns.Detail);
            }
            if (chatGroup.CreatorId == user.Id)
            {
                return Invalid("不能踢群主");
            }
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m.ChatGroupId == chatGroup.Id);
            if (membership == null)
            {
                return NotFound("该成员已不在群聊");
            }
            Context.GroupMemberships.Remove(membership);
            await Context.SaveChangesAsync();
            if (ConnectionService.IsOnline(user.Id))
            {
                await HubContext.Groups.RemoveFromGroupAsync(ConnectionService.GetConnectionId(user.Id), chatGroup.Id.ToString());
            }
            await NotificationService.NotifyGroupMembers(chatGroup.Id, $"{user?.Nickname} 已被移出群聊 {chatGroup?.GroupName}", NotificationTypeEnum.UserKicked);
            await NotificationService.Notify(user.Id, $"您已被移出群聊 {chatGroup?.GroupName}", NotificationTypeEnum.UserKicked);
            return Success("移除群聊成功");
        }

        public Task<ServiceResult<IEnumerable<ChatGroup>>> MyJoinedGroups(User user)
        {
            var groups = Context.GroupMemberships.Include(m => m.User).Include(m => m.ChatGroup).Where(m => m.UserId == user.Id && m.Role != GroupRole.Owner).Select(m => m.ChatGroup).ToList();
            return Task.FromResult(Exist(groups.AsEnumerable(), "查询成功"));
        }

        public Task<ServiceResult<IEnumerable<ChatGroup>>> MyCreatedGroups(User user)
        {
            var groups = Context.GroupMemberships.Include(m => m.User).Include(m => m.ChatGroup).Where(m => m.UserId == user.Id && m.Role == GroupRole.Owner).Select(m => m.ChatGroup).ToList();
            return Task.FromResult(Exist(groups.AsEnumerable(), "查询成功"));
        }

        public async Task<ServiceResult<ChatGroup>> NewGroup(User owner, string groupName)
        {
            var group = new ChatGroup { Creator = owner, GroupName = groupName, CreatorId = owner.Id };
            Context.ChatGroups.Add(group);
            await Context.SaveChangesAsync();
            Context.GroupMemberships.Add(new GroupMembership { ChatGroupId = group.Id, UserId = owner.Id, Role = GroupRole.Owner });
            await Context.SaveChangesAsync();
            return Success(group, "创建群聊成功");
        }

        public async Task<ServiceResult<bool>> OwnsGroup(User user, ChatGroup chatGroup)
        {
            var ismember = await HasMemberShip(user, chatGroup);
            if (!ismember.ExtraData)
            {
                return NotFound(false, ismember.Detail);
            }
            var group = Context.ChatGroups.FirstOrDefault(g => g.Creator.Id == user.Id && chatGroup.Id == g.Id);
            return group != null ? Authorized(true, "您是群主") : Unauthorized(false, "您不是群主");
        }

        public async Task<ServiceResult<object>> QuitGroup(User user, ChatGroup chatGroup)
        {
            var ismember = await HasMemberShip(user, chatGroup);
            if (!ismember.ExtraData)
            {
                return NotFound(ismember.Detail);
            }
            var owns = await OwnsGroup(user, chatGroup);
            if (owns.ExtraData)
            {
                return Invalid(owns.Detail);
            }
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m.ChatGroupId == chatGroup.Id);
            Context.GroupMemberships.Remove(membership);
            await Context.SaveChangesAsync();
            return Success("退群成功");
        }

        public async Task<ServiceResult<object>> RejectJoin(User owner, JoinGroupRequest request)
        {
            var owns = await OwnsGroup(owner, request.Group);
            if (!owns.ExtraData)
            {
                return Unauthorized(owns.Detail);
            }
            request = Context.JoinGroupRequests.FirstOrDefault(r => r.User.Id == request.User.Id && r.Group.Id == request.Group.Id);
            if (request == null)
            {
                return NotFound("该加群申请不存在");
            }
            Context.JoinGroupRequests.Remove(request);
            await Context.SaveChangesAsync();
            return Success("拒绝加群成功");
        }

        public async Task<ServiceResult<object>> SendMessage(User user, ChatGroup chatGroup, ChatMessage.MessageTypeEnum messageType, string content)
        {
            if (messageType == ChatMessage.MessageTypeEnum.Image)
            {
                if (Context.BinaryFiles.FirstOrDefault(f => f.MD5 == content) == null)
                {
                    return NotFound("找不到消息的图片资源，请先上传图片资源");
                }
            }
            ChatMessage chatMessage = new ChatMessage
            {
                MessageOrImageKey = content,
                Type = messageType,
            };
            ChatRecord chatRecord = new ChatRecord
            {
                Sender = user,
                CreatedTime = DateTime.Now,
                Group = chatGroup,
                Message = chatMessage,
            };
            Context.ChatMessages.Add(chatMessage);
            Context.ChatRecords.Add(chatRecord);
            await Context.SaveChangesAsync();
            ChatMessageDetail messageDetail = new ChatMessageDetail
            {
                Id = chatRecord.Id,
                User = user.ToDetail(),
                Group = chatGroup.ToDetail(),
                MessageType = messageType,
                Content = content,
            };
            await NotificationService.NotifyGroupMembers(chatGroup.Id, messageDetail.ToJson(), NotificationTypeEnum.ChatMessage);
            return Success("发送成功");
        }
    }
}
