﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models.QueryJsons;

namespace XueLeMeBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public virtual BinaryFile Avatar { get; set; }
        public virtual ICollection<Authentication> Authentications { get; set; } = new List<Authentication>();
        public virtual ICollection<ScheduleItem> ScheduleItems { get; set; } = new List<ScheduleItem>();
        public UserDetail ToDetail()
        {
            return new UserDetail { Id = Id, Nickname = Nickname, Avatar = Avatar?.MD5 };
        }
    }
}
