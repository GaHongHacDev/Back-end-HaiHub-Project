using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.AppointmentDetailVouchers;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.AppointmentDetailVoucher.AppointmentDetailVouchersEndpoint + "/[action]")]
    [ApiController]
    public class AppointmentDetailVoucherController : ControllerBase
    {
        private readonly IAppointmentDetailVoucherService _appointmentDetailVoucherService;

        public AppointmentDetailVoucherController(IAppointmentDetailVoucherService _appointmentDetailVoucherService)
        {
            this._appointmentDetailVoucherService = _appointmentDetailVoucherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentDetailVouchers(int page=1, int size = 10)
        {
            try
            {
                var schedules = await _appointmentDetailVoucherService.GetAppointmentDetailVouchers(page, size);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentDetailVoucherById(Guid id)
        {
            try
            {
                var schedule = await _appointmentDetailVoucherService.GetAppointmentDetailVoucherById(id);
                if (schedule == null)
                {
                    return NotFound();
                }
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentDetailVoucher(CreateAppointmentDetailVoucherRequest request)
        {
            try
            {
                var isSuccessFull = await _appointmentDetailVoucherService.CreateAppointmentDetailVoucher(request);
                return Ok(isSuccessFull);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAppointmentDetailVoucher(Guid id, UpdateAppointmentDetailVoucherRequest request)
        {
            try
            {
                var isSuccessfull = await _appointmentDetailVoucherService.UpdateAppointmentDetailVoucher(id, request);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
