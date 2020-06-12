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
        public ChatGroupController(IAccountService accountService, IGroupService groupService)
        {
            AccountService = accountService;
            GroupService = groupService;
        }

        public IAccountService AccountService { get; }
        public IGroupService GroupService { get; }

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
        [Route("Join")]
        public async Task<ServiceResult<object>> JoinGroup([FromBody] UserAndGroupForm joinGroupForm)
        {
            var user = await AccountService.UserFromId(joinGroupForm.UserId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result(user.State, user.Detail);
            }
            var group = await GroupService.FromGroupId(joinGroupForm.GroupId);
            if (group.State != ServiceResultEnum.Exist)
            {
                return Result(group.State, group.Detail);
            }
            return await GroupService.JoinGroup(user.ExtraData, group.ExtraData);
        }

        [HttpGet]
        [Route("JoinRequests/{groupId}")]
        public async Task<ServiceResult<IEnumerable<JoinGroupRequest>>> GetJoinRequests(int groupId)
        {
            return await GroupService.JoinGroupRequests(groupId);
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
            var group = await GroupService.FromGroupId(userAndGroupForm.GroupId);
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
            var group = await GroupService.FromGroupId(kickUserForm.OwnerId);
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
            var group = await GroupService.FromGroupId(groupid);
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
            var groups = groupsResult.ExtraData.Select(g => new GroupBrief { Id=g.Id, Name=g.GroupName});
            return Result(groupsResult.State, groups, groupsResult.Detail);
        }
    }
}