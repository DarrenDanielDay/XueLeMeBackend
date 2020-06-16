using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Authentication
    {
        public enum AuthTypeEnum
        {
            MailAddress,
        }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public AuthTypeEnum Type { get; set; }
        public string AuthToken { get; set; }
        public string UserToken { get; set; }
    }
}
