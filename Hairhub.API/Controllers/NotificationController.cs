using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Notification;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Notification.NotificationEndpoint + "/[action]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        public readonly INotificationService _notificationservice;

        public NotificationController(INotificationService notificationService, IMapper mapper) : base(mapper)
        {
            _notificationservice = notificationService;
        }

        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> CreateNotification([FromRoute]Guid id, [FromBody]NotificationRequest request)
        {
            try
            {
                var noti = await _notificationservice.CreatedNotification(id, request);
                return Ok(noti);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetNotification([FromRoute] Guid id, [FromQuery]int page = 1, [FromQuery]int size = 10)
        {
            try
            {
                var noti = await _notificationservice.GetNotification(id, page, size);
                return Ok(noti);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> ReadedNotification([FromRoute] Guid id)
        {
            try
            {
                var noti = await _notificationservice.ReadedNotification(id);
                return Ok(noti);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
