using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.API.Hubs;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Appointment.AppointmentsEndpoint + "/[action]")]
    [ApiController]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IHubContext<BookAppointmentHub> _hubContext;

        public AppointmentController(IMapper mapper, IAppointmentService appointmentService, IHubContext<BookAppointmentHub> hubContext) : base(mapper)
        {
            _appointmentService = appointmentService;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Customer")]
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
                    return NotFound(new { message = "Cannot find this appointment!" });
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
                    return NotFound(new { message = "Cannot find this appointment!" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("{salonId:Guid}")]
        public async Task<IActionResult> GetAppointmentSalonByStatus([FromRoute] Guid salonId, [FromQuery]string? status ,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentSalonByStatus(page, size, salonId, status);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy đơn đặt lịch" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{employeeId:Guid}")]
        public async Task<IActionResult> GetAppointmentEmployeeByStatus([FromRoute] Guid employeeId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentEmployeeByStatus(page, size, employeeId, status);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy đơn đặt lịch" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{customerId:Guid}")]
        public async Task<IActionResult> GetBookingAppointmentCustomer([FromRoute] Guid customerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetBookingAppointmentByCustomerId(page, size, customerId);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy đơn đặt lịch" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest createAppointmentRequest)
        {
            try
            {               
                var accountResponse = await _appointmentService.CreateAppointment(createAppointmentRequest);
                if (accountResponse == false)
                {
                    return NotFound(new { message = "Không thể tạo lịch hẹn" });
                }

                await _hubContext.Clients.All.SendAsync("AppointmentCreated", new
                {
                    Message = "Lịch hẹn đã được đặt thành công",
                    AppointmentDetails = createAppointmentRequest
                });

                return Ok("Tạo lịch hẹn thành công");
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
        public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, [FromBody] UpdateAppointmentRequest updateAppointmentRequest)
        {
            try
            {
                bool isUpdate = await _appointmentService.UpdateAppointmentById(id, updateAppointmentRequest);
                if (!isUpdate)
                {
                    return BadRequest(new {message = "Không thể cập nhật đơn đặt lịch"});
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

        //[HttpPut]
        //[Route("{id:Guid}")]
        //public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        //{
        //    {
        //        try
        //        {
        //            var isDelete = await _appointmentService.DeleteAppoinmentById(id);
        //            if (!isDelete)
        //            {
        //                return BadRequest(new { message = "Cannot delete this appointment!" });
        //            }
        //            return Ok("Delete appointment successfully!");
        //        }
        //        catch (NotFoundException ex)
        //        {
        //            return NotFound(new { message = ex.Message });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { message = ex.Message });
        //        }
        //    }
        //}

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> CancelAppointByCustomer([FromRoute] Guid id, [FromBody] CancelApointmentRequest cancelApointmentRequest)
        {
            {
                try
                {
                    var isDelete = await _appointmentService.CancelAppointmentByCustomer(id, cancelApointmentRequest);
                    if (!isDelete)
                    {
                        return BadRequest(new { message = "Không thể hủy đơn đặt lịch" });
                    }
                    return Ok("Hủy Đơn đặt lịch thành công");
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
        }

        //[HttpPut]
        //[Route("{id:Guid}")]
        //public async Task<IActionResult> ActiveAppointment([FromRoute] Guid id)
        //{
        //    try
        //    {
        //        var isActive = await _appointmentService.ActiveAppointment(id);
        //        if (!isActive)
        //        {
        //            return BadRequest("Cannot delete this appointment!");
        //        }
        //        return Ok("Appointment account successfully!");
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

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
                return NotFound(new { message = ex.Message });
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{SalonId}")]
        public async Task<IActionResult> GetAppointmentBySalonIdNoPaging([FromRoute] Guid SalonId)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetAppointmentSalonBySalonIdNoPaging(SalonId);
                return Ok(appointmentsResponse);
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

        [HttpGet]
        [Route("{customerId:Guid}")]
        public async Task<IActionResult> GetAppointmentCustomerByStatus([FromRoute] Guid customerId, [FromQuery] string? status, [FromQuery] int page, [FromQuery] int size)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetAppointmentCustomerByStatus(customerId, status, page, size);
                return Ok(appointmentsResponse);
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

        [HttpPost]
        public async Task<IActionResult> GetAvailableTime([FromBody] GetAvailableTimeRequest getAvailableTimeRequest)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAvailableTime(getAvailableTimeRequest);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy thời gian phù hợp để thực hiện dịch vụ này" });
                }
                return Ok(appointmentResponse);
            }catch(NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                    return NotFound(new { message = "Không thể tìm thấy voucher này" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                    return NotFound(new { message = "Không tìm thấy thời gian phù hợp để thực hiện dịch vụ này" });
                }
                return Ok(bookingResponse);
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
        [HttpGet]
        
        public async Task<IActionResult> GetAppointmentbyStatusByAdmin([FromQuery]string status, [FromQuery] int year)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetAppointmentbyStatusByAdmin(status, year);
                return Ok(appointmentsResponse);
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
        [HttpGet]
        public async Task<IActionResult> GetRevenueByAdmin([FromQuery]int year)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetRevenueByAdmin(year);
                return Ok(appointmentsResponse);
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

        [HttpGet]
        public async Task<IActionResult> GetCommissionByAdmin([FromQuery] int year)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetCommissionByAdmin(year);
                return Ok(appointmentsResponse);
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

        [HttpGet]
        public async Task<IActionResult> GetAppointmentRatioByStatus([FromQuery] int year)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetPercentagebyStatusOfAppointmentByAdmin(year);
                return Ok(appointmentsResponse);
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
        [HttpGet]
        public async Task<IActionResult> GetAppointmentRatioInYear([FromQuery] int year)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetPercentageOfAppointmentByAdmin(year);
                return Ok(appointmentsResponse);
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

    }
}
