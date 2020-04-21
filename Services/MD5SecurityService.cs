using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public class MD5SecurityService : ISecurityService
    {
        public MD5SecurityService(MD5Service mD5Service)
        {
            MD5Service = mD5Service;
        }

        public MD5Service MD5Service { get; }
        private byte[] buffer = new byte[1024];

        public string Decript(string ciphertext)
        {
            throw new NotImplementedException("Cannot decript MD5");
        }

        public string Encript(string source)
        {
            return MD5Service.MD5OfString(source);
        }

        public string RandomLongString()
        {
            Random random = new Random();
            random.NextBytes(buffer);
            return MD5Service.MD5Generate(buffer);
        }
    }
}
