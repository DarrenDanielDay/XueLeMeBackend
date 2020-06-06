using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class ChangeNicknameForm
    {
        public int UserId { get; set; }
        public string Nickname { get; set; }
    }
}
