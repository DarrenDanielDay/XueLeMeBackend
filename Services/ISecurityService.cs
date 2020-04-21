using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public interface ISecurityService
    {
        public string Encript(string source);
        public string Decript(string ciphertext);
        public string RandomLongString();
    }
}
