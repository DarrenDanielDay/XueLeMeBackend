using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class MailAddressVerifier
    {
        public readonly Regex MailRegex = new Regex("\\w+@\\w+(\\.\\w+)+");
        public async Task<ServiceResult<bool>> IsValidMailAddress(string mail)
        {
            return await Task.FromResult(MailRegex.IsMatch(mail) ? Valid(true, "邮箱格式正确") : Invalid(false, "邮箱格式错误"));
        }
    }
}
