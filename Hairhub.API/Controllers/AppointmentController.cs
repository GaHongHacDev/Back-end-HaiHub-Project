using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Appointment.AppointmentsEndpoint + "/[action]")]
    [ApiController]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IMapper mapper, IAppointmentService appointmentService) : base(mapper)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointment([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var appointmentsResponse = await _appointmentService.GetAllAppointment(page, size);
            return Ok(appointmentsResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentById(id);
                if (appointmentResponse == null)
                {
                    return NotFound("Cannot find this appointment!");
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest createAppointmentRequest)
        {
            try
            {
                var accoutResponse = await _appointmentService.CreateAppointment(createAppointmentRequest);
                if (accoutResponse == null)
                {
                    return BadRequest("Cannot create appointment!");
                }
                //return Ok(accoutResponse);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = accoutResponse.Id }, accoutResponse);
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
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, [FromBody] UpdateAppointmentRequest updateAppointmentRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Apointment Id is null or empty!");
                }

                bool isUpdate = await _appointmentService.UpdateAppointmentById(id, updateAppointmentRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update appointment");
                }
                return Ok("Update appointment successfully");
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
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _appointmentService.DeleteAppoinmentById(id);
                    if (!isDelete)
                    {
                        return BadRequest("Cannot delete this appointment!");
                    }
                    return Ok("Delete appointment successfully!");
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

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> ActiveAppointment([FromRoute] Guid id)
        {
            try
            {
                var isActive = await _appointmentService.ActiveAppointment(id);
                if (!isActive)
                {
                    return BadRequest("Cannot delete this appointment!");
                }
                return Ok("Appointment account successfully!");
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

        [HttpGet]
        [Route("{AccountId:Guid}")]
        public async Task<IActionResult> GetAppointmentByAccountId([FromRoute] Guid AccountId,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetAppointmentByAccountId(AccountId, page, size);
                return Ok(appointmentsResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
