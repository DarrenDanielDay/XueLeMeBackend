using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Models.Fragments;
using XueLeMeBackend.Models.QueryJsons;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatGroupController : ControllerBase
    {
        public ChatGroupController(AccountService accountService, GroupService groupService)
        {
            AccountService = accountService;
            GroupService = groupService;
        }

        public AccountService AccountService { get; }
        public GroupService GroupService { get; }

        [HttpPost]
        [Route("Create")]
        public async Task<ServiceResult<GroupDetail>> CreateGroup([FromBody] CreateGroupForm createGroupForm)
        {
            var result = await AccountService.UserFromId(createGroupForm.UserId);
            if (result.State != ServiceResultEnum.Exist)
            {
                return NotFound<GroupDetail>(null, result.Detail);
            }
            var newGroup = await GroupService.NewGroup(result.ExtraData, createGroupForm.GroupName);
            return newGroup.State == ServiceResultEnum.Success ? Success(newGroup.ExtraData.ToDetail(), newGroup.Detail) : Fail<GroupDetail>(null, newGroup.Detail);
        }

        [HttpPost]
        [Route("ChangeName")]
        public async Task<ServiceResult<object>> ChangeName([FromBody]ChangeGroupNameForm changeGroupNameForm)
        {
            if (!MyForm.ReflectCheck(changeGroupNameForm))
            {
                return Invalid("参数非法");
            }
            var result = await GroupService.GroupFromId(changeGroupNameForm.GroupId);
            if (result.State != ServiceResultEnum.Exist)
            {
                return Result(result.State, result.Detail);
            }
            return await GroupService.ChangeName(result.ExtraData, changeGroupNameForm.NewName);
        }

        [HttpPost]
        [Route("Join")]
        public async Task<ServiceResult<object>> JoinGroup([FromBody] UserAndGroupForm joinGroupForm)
        {
            var user = await AccountService.UserFromId(joinGroupForm.UserId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result(user.State, user.Detail);
            }
            var group = await GroupService.GroupFromId(joinGroupForm.GroupId);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result(group.State, group.Detail);
            }
            return await GroupService.JoinGroup(user.ExtraData, group.ExtraData);
        }

        [HttpGet]
        [Route("JoinRequests/{groupId}")]
        public async Task<ServiceResult<IEnumerable<JoinGroupRequestDetail>>> GetJoinRequests(int groupId)
        {
            var result = await GroupService.JoinGroupRequests(groupId);
            return Result(result.State, result.ExtraData.Select(r => r.ToDetail()), result.Detail);
        }

        [HttpPost]
        [Route("AgreeJoin")]
        public async Task<ServiceResult<object>> AgreeJoin([FromBody] HandleJoinRequestForm handleJoinRequestForm)
        {
            var user = await AccountService.UserFromId(handleJoinRequestForm.UserId);
            var request = await GroupService.JoinRequestFromId(handleJoinRequestForm.RequestId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<object>(user.State, null, user.Detail);
            }
            if (request.State != ServiceResultEnum.Exist)
            {
                return Result<object>(request.State, null, request.Detail);
            }
            return await GroupService.AgreeJoin(user.ExtraData, request.ExtraData);
        }

        [HttpPost]
        [Route("RejectJoin")]
        public async Task<ServiceResult<object>> RejectJoin([FromBody] HandleJoinRequestForm handleJoinRequestForm)
        {
            var user = await AccountService.UserFromId(handleJoinRequestForm.UserId);
            var request = await GroupService.JoinRequestFromId(handleJoinRequestForm.RequestId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<object>(user.State, null, user.Detail);
            }
            if (request.State != ServiceResultEnum.Exist)
            {
                return Result<object>(request.State, null, request.Detail);
            }
            return await GroupService.RejectJoin(user.ExtraData, request.ExtraData);
        }


        [HttpPost]
        [Route("Quit")]
        public async Task<ServiceResult<object>> QuitGroup([FromBody] UserAndGroupForm userAndGroupForm)
        {
            var user = await AccountService.UserFromId(userAndGroupForm.UserId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result(user.State, user.Detail);
            }
            var group = await GroupService.GroupFromId(userAndGroupForm.GroupId);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result(group.State, group.Detail);
            }
            return await GroupService.QuitGroup(user.ExtraData, group.ExtraData);
        }

        [HttpPost]
        [Route("Kick")]
        public async Task<ServiceResult<object>> Kick([FromBody] KickUserForm kickUserForm)
        {
            var owner = await AccountService.UserFromId(kickUserForm.OwnerId);
            var user = await AccountService.UserFromId(kickUserForm.UserId);
            if (owner.State != ServiceResultEnum.Exist || user.State != ServiceResultEnum.Exist)
            {
                return Result(owner.State, owner.Detail);
            }
            var group = await GroupService.GroupFromId(kickUserForm.OwnerId);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result(group.State, group.Detail);
            }
            return await GroupService.KickUser(owner.ExtraData, group.ExtraData, user.ExtraData);
        }

        [HttpGet]
        [Route("Detail/{groupid}")]
        public async Task<ServiceResult<GroupDetail>> Detail(int groupid)
        {
            var group = await GroupService.GroupFromId(groupid);
            if (group.State != ServiceResultEnum.Exist)
            {
                return NotFound<GroupDetail>(null, group.Detail);
            }
            return Exist(group.ExtraData.ToDetail(), "查询成功");
        }

        [HttpGet]
        [Route("MyJoinedGroup/{userid}")]
        public async Task<ServiceResult<IEnumerable<GroupBrief>>> MyJoinedGroups(int userid)
        {
            var user = await AccountService.UserFromId(userid);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<IEnumerable<GroupBrief>>(user.State, null, user.Detail);
            }
            var groupsResult = await GroupService.MyJoinedGroups(user.ExtraData);
            var groups = groupsResult.ExtraData.Select(g => new GroupBrief { Id = g.Id, Name = g.GroupName });
            return Result(groupsResult.State, groups, groupsResult.Detail);
        }

        [HttpGet]
        [Route("MyCreatedGroup/{userid}")]
        public async Task<ServiceResult<IEnumerable<GroupBrief>>> MyCreatedGroups(int userid)
        {
            var user = await AccountService.UserFromId(userid);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<IEnumerable<GroupBrief>>(user.State, null, user.Detail);
            }
            var groupsResult = await GroupService.MyCreatedGroups(user.ExtraData);
            var groups = groupsResult.ExtraData.Select(g => new GroupBrief { Id = g.Id, Name = g.GroupName });
            return Result(groupsResult.State, groups, groupsResult.Detail);
        }
    }
}