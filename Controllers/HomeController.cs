using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;

namespace XueLeMeBackend.Controllers
{
    [Controller]
    public class HomeController : Controller
    {
        public HomeController(IFileService fileService)
        {
            FileService = fileService;
        }

        public IFileService FileService { get; }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("HomePage");
        }
        
    }
}