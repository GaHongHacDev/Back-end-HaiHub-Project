using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.AppointmentDetail.AppointmentDetailsEndpoint + "/[action]")]
    [ApiController]
    public class AppointmentDetailController : BaseController
    {
        private readonly IAppointmentDetailService _appointmentDetailService;

        public AppointmentDetailController(IMapper mapper, IAppointmentDetailService appointmentDetailService) : base(mapper)
        {
            _appointmentDetailService = appointmentDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentDetail([FromQuery] int page, [FromQuery] int size)
        {
            var appointmentsResponse = _appointmentDetailService.GetAllAppointmentDetail(page, size);
            return Ok(appointmentsResponse);
        }
    }
}
