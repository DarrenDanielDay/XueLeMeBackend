using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class ChatRecordService
    {
        public ChatRecordService(XueLeMeContext context)
        {
            Context = context;
        }

        public XueLeMeContext Context { get; }

        public Task<ServiceResult<ChatRecord>> FromRecordId(int id)
        {
            var record = Context.ChatRecords.First(r => r.Id == id);
            return Task.FromResult(record == null ? Exist(record, "查询成功") : NotFound(record, "记录不存在"));
        }

        public Task<ServiceResult<IEnumerable<ChatRecord>>> RecordRightBefore(int groupId, DateTime time, int limit)
        {
            if (limit >= 100)
            {
                return Task.FromResult(Invalid<IEnumerable<ChatRecord>>(null, "查询条数过多"));
            } else if (limit <= 0)
            {
                return Task.FromResult(Invalid<IEnumerable<ChatRecord>>(null, "查询条数应当为正数"));
            }
            var records = Context.ChatRecords.Where(r => r.CreatedTime < time).OrderBy(r => r.CreatedTime).Take(limit);
            return Task.FromResult(Exist(records.AsEnumerable()));
        }

    }
}
