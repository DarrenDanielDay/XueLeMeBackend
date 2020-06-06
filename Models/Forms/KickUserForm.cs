using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class KickUserForm
    {
        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
