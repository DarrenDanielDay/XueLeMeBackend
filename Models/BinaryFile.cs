using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class BinaryFile
    {
        [Key]
        public string MD5 { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedTime { get; set; }
        public byte[] Bytes { get; set; }
    }
}
