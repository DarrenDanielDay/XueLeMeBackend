using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;
namespace XueLeMeBackend.Services
{
    public class AccountService
    {
        public AccountService(XueLeMeContext context)
        {
            Context = context;
        }

        public XueLeMeContext Context { get; }

        public async Task<ServiceResult<object>> ChangeNickname(int userId, string nickname)
        {
            var user = await UserFromId(userId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result<object>(user.State, null, user.Detail);
            }
            user.ExtraData.Nickname = nickname;
            Context.Users.Update(user.ExtraData);
            await Context.SaveChangesAsync();
            return Success("修改昵称成功");
        }

        public Task<ServiceResult<User>> UserFromId(int id)
        {
            var user = Context.Users.Include(u => u.Authentications).FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user == null ? NotFound(user, "用户不存在") : Exist(user));
        }
    }
}
