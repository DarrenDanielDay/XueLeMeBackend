using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Fragments
{
    public class ResetPasswordRequest
    {
        [Key]
        public string EmailAddress { get; set; }
        public string Token { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
