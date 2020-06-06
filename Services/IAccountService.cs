using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;

namespace XueLeMeBackend.Services
{
    public interface IAccountService
    {
        public Task<ServiceResult<User>> UserFromId(int id);
        public Task<ServiceResult<object>> ChangeNickname(int userId, string nickname);
    }
}
