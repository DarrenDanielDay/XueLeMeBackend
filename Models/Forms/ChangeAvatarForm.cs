using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class ChangeAvatarForm
    {
        public int UserId { get; set; }
        public string Avatar { get; set; }
    }
}
