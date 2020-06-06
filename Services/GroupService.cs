using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;
using static XueLeMeBackend.Services.ServiceMessage;
namespace XueLeMeBackend.Services
{
    public class GroupService : IGroupService
    {
        public GroupService(XueLeMeContext context)
        {
            Context = context;
        }

        public XueLeMeContext Context { get; }

        public async Task<ServiceResult<object>> AgreeJoin(User owner, JoinGroupRequest request)
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
            Context.GroupMemberships.Add(new GroupMembership {UserId=request.User.Id, ChatGroupId=request.Group.Id });
            await Context.SaveChangesAsync();
            return Success("通过加群成功");
        }

        public Task<ServiceResult<ChatGroup>> FromGroupId(int id)
        {
            var group = Context.ChatGroups.FirstOrDefault(g => g.Id == id);
            return Task.FromResult(group == null ? NotFound<ChatGroup>(null, "群聊不存在") : Exist(group));
        }

        public Task<ServiceResult<bool>> HasMemberShip(User user, ChatGroup chatGroup)
        {
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m.ChatGroupId == chatGroup.Id);
            return Task.FromResult(membership != null ? Exist(true) : NotFound(false, "该用户不在该群"));
        }

        public async Task<ServiceResult<object>> JoinGroup(User user, ChatGroup chatGroup)
        {
            var request = Context.JoinGroupRequests.FirstOrDefault(r => r.User.Id == user.Id && r.Group.Id == chatGroup.Id);
            if (request != null)
            {
                return Exist("您已经申请加群");
            }
            Context.JoinGroupRequests.Add(new JoinGroupRequest { User = user, Group = chatGroup});
            await Context.SaveChangesAsync();
            return Success("加群申请成功");
        }

        public async Task<ServiceResult<IEnumerable<JoinGroupRequest>>> JoinGroupRequests(int id)
        {
            var group = await FromGroupId(id);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result<IEnumerable<JoinGroupRequest>>(group.State, null, group.Detail);
            }
            return Exist(group.ExtraData.JoinGroupRequests.AsEnumerable(), "查询成功");
        }

        public Task<ServiceResult<JoinGroupRequest>> JoinRequestFromId(int id)
        {
            var request = Context.JoinGroupRequests.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(request == null ? NotFound(request, "加群申请不存在") : Exist(request));
        }

        public async Task<ServiceResult<object>> KickUser(User owner, ChatGroup chatGroup, User user)
        {
            var owns = await OwnsGroup(owner, chatGroup);
            if(!owns.ExtraData)
            {
                return Unauthorized(owns.Detail);
            }
            var membership = Context.GroupMemberships.FirstOrDefault(m => m.UserId == user.Id && m .ChatGroupId == chatGroup.Id);
            if (membership == null)
            {
                return NotFound("该成员已不在群聊");
            }
            Context.GroupMemberships.Remove(membership);
            await Context.SaveChangesAsync();
            return Success();
        }

        public Task<ServiceResult<IEnumerable<ChatGroup>>> MyJoinedGroups(User user)
        {
            var groups = Context.GroupMemberships.Where(m => m.UserId == user.Id).Select(m => m.ChatGroup).ToList();
            return Task.FromResult(Exist(groups.AsEnumerable()));
        }

        public async Task<ServiceResult<ChatGroup>> NewGroup(User owner, string groupName)
        {
            var group = new ChatGroup { Creator = owner, GroupName = groupName };
            Context.ChatGroups.Add(group);
            await Context.SaveChangesAsync();
            Context.GroupMemberships.Add(new GroupMembership { ChatGroupId=group.Id, UserId = owner.Id });
            await Context.SaveChangesAsync();
            return Success(group);
        }

        public Task<ServiceResult<bool>> OwnsGroup(User user, ChatGroup chatGroup)
        {
            var group = Context.ChatGroups.FirstOrDefault(g => g.Creator.Id == user.Id);
            return Task.FromResult(group != null? Authorized(true) : Unauthorized(false, "您不是群主"));
        }

        public async Task<ServiceResult<object>> QuitGroup(User user, ChatGroup chatGroup)
        {
            var ismember = await HasMemberShip(user, chatGroup);
            if (ismember.ExtraData)
            {
                return Exist(ismember.Detail);
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
    }
}
