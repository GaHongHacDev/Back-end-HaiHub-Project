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
                    return NotFound("Appointment detail not found!");
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

      /*  [HttpPost]
        public async Task<IActionResult> CreateAppointmentDetail([FromBody] CreateAppointmentDetailRequest createAppointmentRequest)
        {
            try
            {
                var accoutResponse = await _appointmentDetailService.CreateAppointmentDetail(createAppointmentRequest);
                if (accoutResponse == null)
                {
                    return BadRequest("Cannot create appointment!");
                }
                return Ok(accoutResponse);
            }
            catch (NotFoundException ex)
            {

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAppointmentDetail([FromRoute] Guid id, [FromBody] UpdateAppointmentDetailRequest updateAppointmentDetailRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Apointment Id is null or empty!");
                }

                bool isUpdate = await _appointmentDetailService.UpdateAppointmentDetailById(id, updateAppointmentDetailRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update appointment detail");
                }
                return Ok("Update appointment detail successfully");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                        return BadRequest("Cannot delete this appointment!");
                    }
                    return Ok("Delete appointment detail successfully!");
                }
                catch (NotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
