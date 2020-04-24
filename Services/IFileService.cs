using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Models;

namespace XueLeMeBackend.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Save the given file and return a binary file entity.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ServiceResult<BinaryFile>> SaveFile(IFormFile file);
        /// <summary>
        /// Get the file with given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<ServiceResult<BinaryFile>> GetFile(string name);
    }
}
