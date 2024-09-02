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
        private readonly IHubContext<BookAppointmentHub, IBookAppointmentHub> _hubContext;

        public SignalRHubController(IHubContext<BookAppointmentHub, IBookAppointmentHub> hubContext, IMapper mapper) : base(mapper)   
        {
            _hubContext = hubContext;
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastMessage([FromBody] string message)
        {
            await _hubContext.Clients.All.RecieveMessage(message);
            return NoContent();
        }
    }
}
