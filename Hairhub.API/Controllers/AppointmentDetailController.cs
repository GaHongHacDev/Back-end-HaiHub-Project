using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
   /* [Route(ApiEndPointConstant.AppointmentDetail.AppointmentDetailsEndpoint + "/[action]")]
    [ApiController]
    public class AppointmentDetailController : BaseController
    {
        private readonly IAppointmentDetailService _appointmentDetailService;

        public AppointmentDetailController(IMapper mapper, IAppointmentDetailService appointmentDetailService) : base(mapper)
        {
            _appointmentDetailService = appointmentDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentDetail([FromQuery]int page=1, [FromQuery] int size = 10)
        {
            var appointmentsResponse = await _appointmentDetailService.GetAllAppointmentDetail(page, size);
            return Ok(appointmentsResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentDetailById([FromRoute] Guid id)
        {
            try
            {
                var appointmentResponse = await _appointmentDetailService.GetAppointmentDetailById(id);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy chi tiết đơn đặt lịch" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAppointmentDetail([FromRoute] Guid id, [FromBody] UpdateAppointmentDetailRequest updateAppointmentDetailRequest)
        {
            try
            {
                bool isUpdate = await _appointmentDetailService.UpdateAppointmentDetailById(id, updateAppointmentDetailRequest);
                if (!isUpdate)
                {
                    return BadRequest(new {message = "Không thể cập nhật chi tiết đơn đặt lịch" });
                }
                return Ok("Cập nhật đơn đặt lịch thành công");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteAppointmentDetail([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _appointmentDetailService.DeleteAppoinmentDetailById(id);
                    if (!isDelete)
                    {
                        return BadRequest(new { message = "Không thể xóa chi tiết đặt lịch" });
                    }
                    return Ok("Delete appointment detail successfully!");
                }
                catch (NotFoundException ex)
                {
                    return NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
*/
}
