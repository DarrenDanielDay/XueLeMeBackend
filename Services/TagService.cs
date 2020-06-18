using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;
namespace XueLeMeBackend.Services
{
    public class TagService
    {
        public TagService(XueLeMeContext context)
        {
            Context = context;
        }

        public XueLeMeContext Context { get; }

        public async Task<ServiceResult<Tag>> Create(string tag)
        {
            var existingTag = Context.Tags.FirstOrDefault(t => t.DisplayName == tag);
            if (existingTag != null)
            {
                return Exist(existingTag, "该Tag已存在");
            }
            var newtag = new Tag { DisplayName = tag };
            Context.Tags.Add(newtag);
            await Context.SaveChangesAsync();
            return Success(newtag, "创建成功");
        }

        public async Task<ServiceResult<object>> CreateRange(ICollection<string> tags)
        {
            int count = 0;
            foreach (var tag in tags)
            {
                if ((await Create(tag)).State != ServiceResultEnum.Exist)
                {
                    count++;
                }
            }
            return Success($"成功创建了{count}个Tag");
        }

        public Task<ServiceResult<bool>> TagExist(string tag)
        {
            return Task.FromResult(Context.Tags.FirstOrDefault(t => t.DisplayName == tag) == null ? NotFound(false, "Tag不存在") : Exist(true, "Tag已存在"));
        }

        public Task<ServiceResult<ICollection<Tag>>> TagsFromStrings(ICollection<string> strings)
        {
            ICollection<Tag> tags = new List<Tag>();
            foreach (var str in strings)
            {
                var tag = Context.Tags.FirstOrDefault(t => t.DisplayName == str);
                if (tag != null)
                {
                    tags.Add(tag);
                }
            }
            return Task.FromResult(tags.Count == strings.Count ? Exist(tags, "查询成功") : NotFound(tags, "部分未找到"));
        }
    }
}
