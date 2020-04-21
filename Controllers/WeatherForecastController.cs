using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMailService mainService)
        {
            _logger = logger;
            MainService = mainService;
        }

        public IMailService MainService { get; }

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
    }
}
