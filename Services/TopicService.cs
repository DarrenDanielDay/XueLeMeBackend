using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;
using XueLeMeBackend.Models;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Services
{
    public class TopicService
    {
        public TopicService(XueLeMeContext context, TagService tagService)
        {
            Context = context;
            TagService = tagService;
        }

        public XueLeMeContext Context { get; }
        public TagService TagService { get; }

        public ICollection<BinaryFile> CheckFiles(ICollection<string> files)
        {
            ICollection<BinaryFile> binaryFiles = new List<BinaryFile>();

            foreach (var item in files)
            {
                var file = Context.BinaryFiles.FirstOrDefault(f => f.MD5 == item);
                if (file == null)
                {
                    break;
                }
                else
                {
                    binaryFiles.Add(file);
                }
            }
            return binaryFiles.Count == files.Count ? binaryFiles : null;
        }

        public async Task<ServiceResult<TextAndImageContent>> CreateContent(string content, ICollection<string> images)
        {
            ICollection<BinaryFile> binaryFiles = CheckFiles(images);
            if (binaryFiles == null)
            {
                return NotFound<TextAndImageContent>(null, "找不到图片");
            }
            else
            {
                TextAndImageContent textAndImageContent = new TextAndImageContent
                {
                    Text = content,
                };
                Context.TextAndImageContents.Add(textAndImageContent);
                await Context.SaveChangesAsync();
                Context.AdditionalImages.AddRange(binaryFiles.Select(
                    f => new AdditionalImage
                    {
                        ImageFileMD5 = f.MD5,
                        TextAndImageContentId = textAndImageContent.Id,
                        ImageFile = f,
                        TextAndImageContent = textAndImageContent
                    }
                    ));
                await Context.SaveChangesAsync();
                return Success(textAndImageContent, "创建成功");
            }
        }

        public async Task<ServiceResult<Zone>> CreateZone(string name)
        {
            var exisiting = Context.Zones.FirstOrDefault(z => z.Name == name);
            if (exisiting != null)
            {
                return Exist(exisiting, "该论坛名已存在");
            }
            var zone = new Zone
            {
                Name = name,
            };
            Context.Zones.Add(zone);
            await Context.SaveChangesAsync();
            return Success(zone, "创建论坛成功");
        }

        public Task<ServiceResult<Zone>> ZoneFromId(int id)
        {
            var zone = Context.Zones.FirstOrDefault(z => z.Id == id);
            return Task.FromResult(zone == null ? NotFound<Zone>(null, "该论坛不存在") : Exist(zone, "查询成功"));
        }

        public async Task<ServiceResult<Topic>> Create(string title, User publisher, Zone zone, ICollection<Tag> tags, string content, ICollection<string> images)
        {
            var createdContent = await CreateContent(content, images);
            if (createdContent.State != ServiceResultEnum.Success)
            {
                return Result<Topic>(createdContent.State, null, createdContent.Detail);
            }

            var topic = new Topic
            {
                Content = createdContent.ExtraData,
                Publisher = publisher,
                Title = title,
                Zone = zone,
                ZoneId = zone.Id,
                ContentId = createdContent.ExtraData.Id,
                PublisherId = publisher.Id,
            };
            Context.Topics.Add(topic);
            await Context.SaveChangesAsync();
            var appliedTags = tags.Select(t =>
            new AppliedTag
            {
                Tag = t,
                TagDisplayName = t.DisplayName,
                Topic = topic,
                TopicId = topic.Id
            });
            Context.AppliedTags.AddRange(appliedTags);
            await AnonymousOfUser(publisher, topic);

            await Context.SaveChangesAsync();
            return Success(topic, "创建话题成功");

        }

        public ServiceResult<TextAndImageContent> TextAndImageContentFromId(int id)
        {
            var content = Context.TextAndImageContents
                .Include(t => t.Images)
                .FirstOrDefault(t => t.Id == id);
            return content == null ? NotFound(content) : Exist(content);
        }

        public Task<ServiceResult<Topic>> TopicFromId(int id)
        {
            var topic = Context.Topics
                .Include(t => t.AppliedTags)
                .Include(t => t.Content)
                .Include(t => t.Publisher)
                .Include(t => t.Replies)
                .Include(t => t.Zone)
                .Include(t => t.Anonymous)
                .FirstOrDefault(t => t.Id == id);

            if (topic != null)
            {
                topic.Content = TextAndImageContentFromId(topic.ContentId).ExtraData;
                topic.Replies = Context.Replies.Include(r => r.Content).Where(r => r.TopicId == topic.Id).ToList();
            }

            return Task.FromResult(topic == null ? NotFound(topic, "话题不存在") : Exist(topic, "查询成功"));

        }

        public Task<ServiceResult<Reply>> ReplyFromId(int id)
        {
            var reply = Context.Replies
                .Include(r => r.Content)
                .Include(r => r.Reference)
                .Include(r => r.Responder)
                .Include(r => r.Topic)
                .FirstOrDefault();
            if (reply != null)
            {
                reply.Content = TextAndImageContentFromId(reply.ContentId).ExtraData;
            }
            return Task.FromResult(reply == null ? NotFound(reply, "回复不存在") : Exist(reply, "查询成功"));
        }

        public async Task<ServiceResult<Reply>> MakeReply(User user, Topic topic, Reply reference, string content, ICollection<string> images)
        {
            var createdContent = await CreateContent(content, images);
            if (createdContent.State != ServiceResultEnum.Success)
            {
                return Result<Reply>(createdContent.State,null, createdContent.Detail);
            }
            else
            {
                Reply reply = new Reply
                {
                    Content = createdContent.ExtraData,
                    ContentId = createdContent.ExtraData.Id,
                    Reference = reference,
                    Responder = user,
                    ResponderId = user.Id,
                    Topic = topic,
                    TopicId = topic.Id
                };
                Context.Replies.Add(reply);
                await AnonymousOfUser(user, topic);
                await Context.SaveChangesAsync();
                return Success(reply, "回复成功");
            }

        }

        public async Task<ServiceResult<Anonymous>> AnonymousOfUser(User user, Topic topic)
        {
            var anonymous = Context.Anonymous.FirstOrDefault(a => a.UserId == user.Id && a.TopicId == topic.Id);
            if (anonymous == null)
            {
                var existingNames = Context.Anonymous.Where(a => a.TopicId == topic.Id).Select(a => a.DisplayName).ToList();
                anonymous = new Anonymous
                {
                    Topic = topic,
                    TopicId = topic.Id,
                    User = user,
                    UserId = user.Id,
                    DisplayName = GenerateAnonymous(existingNames)
                };
                Context.Anonymous.Add(anonymous);
                await Context.SaveChangesAsync();
                
            }
            return Success(anonymous, "查询成功");
        }

        public string GenerateAnonymous(ICollection<string> excludes)
        {
            Random random = new Random();
            string result;
            do
            {
                byte[] bytes = new byte[6];
                random.NextBytes(bytes);
                result = Convert.ToBase64String(bytes).Replace('+', '0').Replace('-', '1');
            } while (excludes.Any(e => e == result));
            return result;
        }


    }
}
