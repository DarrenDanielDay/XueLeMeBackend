using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Services;
using XueLeMeBackend.Models.QueryJsons;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRecordController : ControllerBase
    {
        public ChatRecordController(ChatRecordService chatRecordService)
        {
            ChatRecordService = chatRecordService;
        }

        public ChatRecordService ChatRecordService { get; }

        [HttpGet]
        [Route("Detail/{recordId}")]
        public async Task<ServiceResult<ChatRecordDetail>> Detail(int recordId)
        {
            var result = await ChatRecordService.FromRecordId(recordId);
            if (result.State != ServiceResultEnum.Exist)
            {
                return Result<ChatRecordDetail>(result.State, null, result.Detail);
            }
            return Exist(result.ExtraData.ToDetail(), result.Detail);
        }

        [HttpGet]
        [Route("RecordsBeforeTime")]
        public async Task<ServiceResult<IEnumerable<ChatRecordDetail>>> QueryTimeBefore(int groupId, DateTime time, int limit)
        {
            var results = await ChatRecordService.RecordRightBefore(groupId, time, limit);
            if (results.State != ServiceResultEnum.Exist)
            {
                return Result<IEnumerable<ChatRecordDetail>>(results.State, null, results.Detail);
            }
            return Exist(results.ExtraData.Select(r => r.ToDetail()).AsEnumerable());
        }
    }
}