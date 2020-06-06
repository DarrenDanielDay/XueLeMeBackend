using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Fragments;

namespace XueLeMeBackend.Services
{
    public interface IGroupService
    {
        public Task<ServiceResult<ChatGroup>> NewGroup(User owner, string groupName);
        public Task<ServiceResult<ChatGroup>> FromGroupId(int id);
        public Task<ServiceResult<bool>> OwnsGroup(User user, ChatGroup chatGroup);
        public Task<ServiceResult<bool>> HasMemberShip(User user, ChatGroup chatGroup);
        public Task<ServiceResult<IEnumerable<ChatGroup>>> MyJoinedGroups(User user);
        public Task<ServiceResult<object>> AgreeJoin(User owner, JoinGroupRequest request);
        public Task<ServiceResult<object>> RejectJoin(User owner, JoinGroupRequest request);
        public Task<ServiceResult<object>> JoinGroup(User user, ChatGroup chatGroup);
        public Task<ServiceResult<object>> QuitGroup(User user, ChatGroup chatGroup);
        public Task<ServiceResult<object>> KickUser(User owner, ChatGroup chatGroup, User user);
        public Task<ServiceResult<JoinGroupRequest>> JoinRequestFromId(int id);
        public Task<ServiceResult<IEnumerable<JoinGroupRequest>>> JoinGroupRequests(int id);
    }
}
