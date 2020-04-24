using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;

namespace XueLeMeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        public IFileService FileService { get; }

        public FileController(IFileService fileService)
        {
            FileService = fileService;
        }

        [HttpPost]
        [Route("PostFile")]
        public async Task<ServiceResult<BinaryFile>> PostFile(IFormFile file)
        {
            if (file == null)
            {
                return ServiceMessage.Invalid<BinaryFile>(null, "未上传任何文件");
            }
            var result = await FileService.SaveFile(file);
            if (result.ExtraData != null)
            {
                result.ExtraData = MyForm.Clone(result.ExtraData);
                result.ExtraData.Bytes = null;
            }
            return result;
        }

        [HttpGet]
        [Route("Files/{filename}")]
        public async Task<IActionResult> GetFile(string filename)
        {
            var result = await FileService.GetFile(filename);
            if (result.State == ServiceResultEnum.NotFound)
            {
                return NotFound();
            }
            return File(result.ExtraData.Bytes, result.ExtraData.ContentType, result.ExtraData.FileName);
        }
    }
}