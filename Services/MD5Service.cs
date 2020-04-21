using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public class MD5Service
    {
        public MD5 MD5Instance { get; set; }

        public MD5Service()
        {
            MD5Instance = new MD5CryptoServiceProvider();
        }

        public string MD5OfString(string sourceString)
        {
            var sourceBytes = System.Text.Encoding.UTF8.GetBytes(sourceString);
            return MD5Generate(sourceBytes);
        }


        public string MD5Generate(byte[] bytes)
        {
            var targetBytes = MD5Instance.ComputeHash(bytes);
            string result = null;
            foreach (byte b in targetBytes)
            {
                result += b.ToString("x2");
            }
            return result;
        }
    }
}
