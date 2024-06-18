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

        [HttpGet]
        [Route("{customerId:Guid}")]
        public async Task<IActionResult> GetHistoryAppointmentByCustomterId([FromRoute] Guid customerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetHistoryAppointmentByCustomerId(page, size,customerId);
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

        [HttpGet]
        [Route("{customerId:Guid}")]
        public async Task<IActionResult> GetBookingAppointment([FromRoute] Guid customerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetBookingAppointment(page, size, customerId);
                if (appointmentResponse == null)
                {
                    return NotFound("Không tìm thấy đơn đặt lịch");
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
                if (accoutResponse == false)
                {
                    return BadRequest("Không thể tạo lịch hẹn");
                }
                return Ok("Tạo lịch hẹn thành công");
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

        [HttpPost]
        public async Task<IActionResult> GetAvailableTime([FromBody] GetAvailableTimeRequest getAvailableTimeRequest)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAvailableTime(getAvailableTimeRequest);
                if (appointmentResponse == null)
                {
                    return NotFound("Không tìm thấy thời gian phù hợp để thực hiện dịch vụ này");
                }
                return Ok(appointmentResponse);
            }catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CalculatePrice([FromBody]GetCalculatePriceRequest request)
        {            
            try
            {
                var result = await _appointmentService.CalculatePrice(request);
                if (result == null)
                {
                    return NotFound("Không thể tìm thấy voucher này");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest bookAppointmentRequest)
        {
            try
            {
                var bookingResponse = await _appointmentService.BookAppointment(bookAppointmentRequest);
                if (bookingResponse == null)
                {
                    return NotFound("Không tìm thấy thời gian phù hợp để thực hiện dịch vụ này");
                }
                return Ok(bookingResponse);
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
