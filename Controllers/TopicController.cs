using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.Forms;
using XueLeMeBackend.Models.QueryJsons;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        public TopicController(TopicService topicService, AccountService accountService, TagService tagService)
        {
            TopicService = topicService;
            AccountService = accountService;
            TagService = tagService;
        }

        public TopicService TopicService { get; }
        public AccountService AccountService { get; }
        public TagService TagService { get; }

        [HttpPost]
        [Route("CreateZone")]
        public async Task<ServiceResult<object>> CreateZone([FromBody] CreateZoneForm createZoneForm)
        {
            var result = await TopicService.CreateZone(createZoneForm.ZoneName);
            return Result(result.State, result.Detail);
        }

        [HttpPost]
        [Route("CreateTopic")]
        public async Task<ServiceResult<object>> CreateTopic([FromBody] CreateTopicForm createTopicForm)
        {
            var zone = await TopicService.ZoneFromId(createTopicForm.ZoneId);
            if (zone.State != ServiceResultEnum.Exist)
            {
                return Result(zone.State, zone.Detail);
            }
            var user = await AccountService.UserFromId(createTopicForm.UserId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result(user.State, user.Detail);
            }
            var tags = await TagService.TagsFromStrings(createTopicForm.Tags);
            if (tags.State != ServiceResultEnum.Exist)
            {
                return Result(tags.State, tags.Detail);
            }
            var result = await TopicService.Create(createTopicForm.Title, user.ExtraData, zone.ExtraData, tags.ExtraData,createTopicForm.Content,createTopicForm.Images);
            return Result(result.State, result.Detail);
        }

        [HttpPost]
        [Route("MakeReply")]
        public async Task<ServiceResult<object>> Reply([FromBody] MakeReplyForm makeReplyForm)
        {
            var user =await  AccountService.UserFromId(makeReplyForm.UserId);
            if (user.State != ServiceResultEnum.Exist)
            {
                return Result(user.State, user.Detail);
            }
            var topic = await TopicService.TopicFromId(makeReplyForm.TopicId);
            if (topic.State != ServiceResultEnum.Exist)
            {
                return Result(topic.State, topic.Detail);
            }
            Reply reply = null;
            if (makeReplyForm.ReferenceId != null)
            {
                var _reply = await TopicService.ReplyFromId(makeReplyForm.ReferenceId ?? 0);
                if (_reply.State != ServiceResultEnum.Exist)
                {
                    return Result(_reply.State, _reply.Detail);
                } else
                {
                    reply = _reply.ExtraData;
                }
            }
            var result = await TopicService.MakeReply(user.ExtraData, topic.ExtraData, reply, makeReplyForm.Content, makeReplyForm.Images);
            return result;
        }

        [HttpGet]
        [Route("TopicDetail/{topicId}")]
        public async Task<ServiceResult<TopicDetail>> TopicDetail(int topicId)
        {
            var topic = await TopicService.TopicFromId(topicId);
            if (topic.State != ServiceResultEnum.Exist)
            {
                return Result<TopicDetail>(topic.State, null, topic.Detail);
            }
            return Success(topic.ExtraData.ToDetail(), topic.Detail);
        }

        [HttpGet]
        [Route("ReplyDetail/{replyid}")]
        public async Task<ServiceResult<ReplyDetail>> ReplyDetail(int replyid)
        {
            var reply = await TopicService.ReplyFromId(replyid);
            if (reply.State != ServiceResultEnum.Exist)
            {
                return Result<ReplyDetail>(reply.State, null, reply.Detail);
            }
            var topic = await TopicService.TopicFromId(reply.ExtraData.TopicId);
            
            var re = reply.ExtraData;
            re.Topic = topic.ExtraData;
            return Exist(re.ToDetail(), "查询成功");

        }

        [HttpGet]
        [Route("RepliesOfTopic/{topicid}")]
        public async Task<ServiceResult<ICollection<ReplyDetail>>> RepliesOfTopic(int topicid)
        {
            var topic = await TopicService.TopicFromId(topicid);
            if (topic.State != ServiceResultEnum.Exist)
            {
                return Result<ICollection<ReplyDetail>>(topic.State, null, topic.Detail);
            }
            ICollection<ReplyDetail> replyDetails = new List<ReplyDetail>();
            foreach(var reply in topic.ExtraData.Replies)
            {
                var detailedReply = await TopicService.ReplyFromId(reply.Id);
                replyDetails.Add(detailedReply.ExtraData.ToDetail());
            }
            return Exist(replyDetails, "查询成功");
        }

        [HttpGet]
        [Route("AllZones")]
        public ServiceResult<ICollection<ZoneDetail>> AllZones()
        {
            ICollection<ZoneDetail> zones = TopicService.Context.Zones.Select(z => z.ToDetail()).ToList();
            return Exist(zones, "查询成功");
        }

        [HttpGet]
        [Route("AllTopics/{zoneid}")]
        public async Task<ServiceResult<ICollection<TopicDetail>>> AllTopics(int zoneid)
        {
            var c = TopicService.Context;
            var zone = await TopicService.ZoneFromId(zoneid);
            if (zone.State != ServiceResultEnum.Exist)
            {
                return Result<ICollection<TopicDetail>>(zone.State, null, zone.Detail);
            }
            ICollection<Topic> topics = c.Topics.Where(t => t.ZoneId == zoneid).ToList();
            ICollection<TopicDetail> topicDetails = new List<TopicDetail>();
            foreach(var topic in topics)
            {
                var detailed = await TopicService.TopicFromId(topic.Id);
                topicDetails.Add(detailed.ExtraData.ToDetail());
            }
            return Exist(topicDetails, "查询成功");
        }
    }
}