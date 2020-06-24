using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class DbFileService : IFileService
    {
        public DbFileService(XueLeMeContext xueLeMeContext, MD5Service mD5Service, ILogger<DbFileService> logger)
        {
            Context = xueLeMeContext;
            MD5Service = mD5Service;
            Logger = logger;
        }

        public XueLeMeContext Context { get; }
        public MD5Service MD5Service { get; }
        public ILogger Logger { get; }

        public async Task<ServiceResult<BinaryFile>> GetFile(string name)
        {
            var file = await Context.BinaryFiles.FirstOrDefaultAsync(f => f.MD5 == name);
            if (file == null)
            {
                return NotFound(file, "文件不存在");
            }
            else
            {
                return Exist(file, "文件存在");
            }
        }

        public async Task<ServiceResult<BinaryFile>> SaveFile(IFormFile file)
        {
            if (file.Length > 1 << 20)
            {
                return Fail<BinaryFile>(null, "文件过大");
            }
            var binaryFile = new BinaryFile {
                Bytes = new byte[file.Length],
                ContentType = file.ContentType, 
                FileName = file.FileName 
            };
            using (var fileStream = file.OpenReadStream())
            {
                await fileStream.ReadAsync(binaryFile.Bytes, 0, Convert.ToInt32(file.Length));
            }
            binaryFile.MD5 = MD5Service.MD5Generate(binaryFile.Bytes);
            var existfiles = Context.BinaryFiles.Where(f => f.MD5.StartsWith(binaryFile.MD5));
            if (existfiles.Any())
            {

                foreach (var existfile in existfiles)
                {
                    // compare the actual bytes...
                    if (!BytesCompare(existfile.Bytes, binaryFile.Bytes))
                    {
                        // unluckily, we had to add time suffix...
                        binaryFile.MD5 += DateTime.Now.ToString();
                        Logger.Log(LogLevel.Debug, "Congratulations! We found file md5 crash in two different files!");
                    }
                    else
                    {
                        return Exist(existfile, "相同文件已存在");
                    }
                }
            }
            Context.BinaryFiles.Add(binaryFile);
            await Context.SaveChangesAsync();
            return Success(binaryFile, "文件保存成功");
        }

        private bool BytesCompare(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (var i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
