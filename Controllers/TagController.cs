using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        public TagController(TagService tagService)
        {
            TagService = tagService;
        }

        public TagService TagService { get; }

        [HttpPost]
        [Route("Create")]
        public async Task<ServiceResult<object>> Create([FromBody] TagForm tagForm)
        {
            var result = await TagService.Create(tagForm.tag);
            return Result(result.State, result.Detail);
        }

        [HttpPost]
        [Route("CreateMany")]
        public async Task<ServiceResult<object>> CreateMany([FromBody] ICollection<string> tags)
        {
            var result = await TagService.CreateRange(tags);
            return Result(result.State, result.Detail);
        }

        [HttpGet]
        [Route("All")]
        public ServiceResult<IEnumerable<string>> AllTags()
        {
            var tags = TagService.Context.Tags.Select(t => t.DisplayName).AsEnumerable();
            return Exist(tags, "查询成功");
        }
    }
}