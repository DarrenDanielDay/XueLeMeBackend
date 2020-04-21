using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public interface IMailService
    {   
        public Task<ServiceResult<bool>> IsValidMailAddress(string mail);
        public Task<ServiceResult<bool>> SendMail(string to, string title, string content);
    }
}
