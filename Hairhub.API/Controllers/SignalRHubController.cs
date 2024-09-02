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
        public async Task<IActionResult> BroadcastMessage([FromBody] string message)
        {
            var timestamp = DateTime.Now;
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message, timestamp);
            return NoContent();
        }
    }
}
