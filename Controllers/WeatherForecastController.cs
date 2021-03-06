﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using XueLeMeBackend.Data;
using XueLeMeBackend.Hubs;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;

namespace XueLeMeBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public WeatherForecastController(IMailService mainService, XueLeMeContext context, IHubContext<ChatHub> hubContext)
        {
            MainService = mainService;
            Context = context;
            HubContext = hubContext;
        }

        public IMailService MainService { get; }
        public XueLeMeContext Context { get; }
        public IHubContext<ChatHub> HubContext { get; }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        [Route("MailTest")]
        public async Task<ServiceResult<object>> MailTest()
        {
            var result = await MainService.SendMail("614434935@qq.com", "QQ邮箱测试", "这是一段内容");
            return result.ExtraData ? Success() : Fail();
        }

        [HttpGet]
        [Route("HubTest")]
        public async Task<IActionResult> HubTest()
        {
            var targets = HubContext.Clients.All;
            await targets.SendAsync("OnReceiveMessage", 1, 1, 1, "HubTest", DateTime.Now);
            return Ok();
        }
    }
}
