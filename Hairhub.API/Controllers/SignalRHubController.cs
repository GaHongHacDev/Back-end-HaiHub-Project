using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.SignalR.SignalRsEndpoint+ "/[action]")]
    [ApiController]
    public class SignalRHubController : BaseController
    {
        private readonly IHubContext<BookAppointmentHub> _hubContext;

        public SignalRHubController(IHubContext<BookAppointmentHub> hubContext, IMapper mapper) : base(mapper)   
        {
            _hubContext = hubContext;
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastMessage([FromBody] BroadcastMessageRequest request)
        {
            var timestamp = DateTime.Now;
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.Message, request.DateAppointment, timestamp, request.SalonId, request.ServiceId, request.OwnerId);
            return NoContent();
        }
    }
    public class BroadcastMessageRequest
    {
        public string Message { get; set; }

        public string DateAppointment { get; set; }
        public string SalonId { get; set; }
        public List<string> ServiceId { get; set; }

        public string OwnerId { get; set; }
    }
}
