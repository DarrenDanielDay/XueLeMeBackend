using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XueLeMeBackend.Models;
using XueLeMeBackend.Models.QueryJsons;
using XueLeMeBackend.Services;
using static XueLeMeBackend.Services.ServiceMessage;
namespace XueLeMeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public NotificationController(NotificationService notificationService)
        {
            NotificationService = notificationService;
        }

        public NotificationService NotificationService { get; }

        [HttpGet]
        [Route("MyNotifications/{userid}")]
        public async Task<ServiceResult<IEnumerable<NotificationDetail>>> MyNotifications(int userid)
        {
            var notifications = await NotificationService.GetNotifications(userid);
            return Exist(notifications.Select(n => n.ToDetail()));
        }

        [HttpDelete]
        [Route("Read/{id}")]
        public async Task<ServiceResult<object>> ReadNotification(int id)
        {
            return await NotificationService.ReadNotification(id);
        }

        [HttpDelete]
        [Route("ReadMany")]
        public async Task<ServiceResult<object>> ReadNotifications([FromBody] IEnumerable<int> ids)
        {
            return await NotificationService.ReadNotifications(ids);
        }
    }
}