using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class Tag
    {
        [Key]
        public string DisplayName { get; set; }
    }
}
