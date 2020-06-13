using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models.Forms
{
    public class ChangeGroupNameForm
    {
        public int GroupId { get; set; }
        public string NewName { get; set; }
    }
}
