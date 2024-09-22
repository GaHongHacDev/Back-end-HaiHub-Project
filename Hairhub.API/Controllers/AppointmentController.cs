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

        public AppointmentController(IMapper mapper, IAppointmentService appointmentService) : base(mapper)
        {
            _appointmentService = appointmentService;

        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetAllAppointment([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var appointmentsResponse = await _appointmentService.GetAllAppointment(page, size);
            return Ok(appointmentsResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{customerId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{SalonId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetAppointmentTransaction([FromRoute] Guid SalonId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentTransaction(SalonId, startDate, endDate);
                if (appointmentResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy transaction" });
                }
                return Ok(appointmentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{salonId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetAppointmentSalonByStatus([FromRoute] Guid salonId, [FromQuery]string? status, [FromQuery] bool isAscending, [FromQuery] DateTime? date, [FromQuery] string? customerName,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentSalonByStatus(page, size, salonId, status, isAscending, date, customerName);
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
        [Route("{employeeId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> GetAppointmentEmployeeByStatus([FromRoute] Guid employeeId, [FromQuery] string? status, [FromQuery] bool isAscending, [FromQuery] DateTime? date, [FromQuery] string? customerName, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentResponse = await _appointmentService.GetAppointmentEmployeeByStatus(employeeId, page, size, status, isAscending, date, customerName);
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

        //[HttpGet]
        //[Route("{employeeId:Guid}")]
        //[Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        //public async Task<IActionResult> GetAppointmentEmployeeByStatus([FromRoute] Guid employeeId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int size = 10)
        //{
        //    try
        //    {
        //        var appointmentResponse = await _appointmentService.GetAppointmentEmployeeByStatus(page, size, employeeId, status);
        //        if (appointmentResponse == null)
        //        {
        //            return NotFound(new { message = "Không tìm thấy đơn đặt lịch" });
        //        }
        //        return Ok(appointmentResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpGet]
        [Route("{customerId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest createAppointmentRequest)
        {
            try
            {               
                var accountResponse = await _appointmentService.CreateAppointment(createAppointmentRequest);
                if (accountResponse == false)
                {
                    return NotFound(new { message = "Không thể tạo lịch hẹn" });
                }

                //await _hubContext.Clients.All.SendAsync("AppointmentCreated", new
                //{
                //    Message = "Lịch hẹn đã được đặt thành công",
                //    AppointmentDetails = createAppointmentRequest
                //});

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
        [Authorize(Roles = RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Customer)]
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

        [HttpGet]
        [Route("{AccountId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetAppointmentCustomerByStatus([FromRoute] Guid customerId, [FromQuery] string? status, [FromQuery] bool isAscending, 
                                                                          [FromQuery] DateTime? date, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.GetAppointmentCustomerByStatus(customerId, status, isAscending, date, page, size);
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
        [Authorize(Roles = RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Customer)]
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
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

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> RevenueandNumberofAppointment([FromRoute] Guid id, [FromQuery]DateTime? startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.RevenueandNumberofAppointment(id, startdate, enddate);
                var formattedResponse = new
                {
                    TotalRevenue = appointmentsResponse.Item1,
                    TotalAppointmentSuccessed = appointmentsResponse.Item2,
                    
                };
                return Ok(formattedResponse);
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
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> RateAppointmentByStatus([FromRoute] Guid id, [FromQuery] DateTime? startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.RateofAppointmentByStatus(id, startdate, enddate);
                var formattedResponse = new
                {
                    SuccessedRate = appointmentsResponse.Item1,
                    FailedRate = appointmentsResponse.Item2,
                    CancelByCustomerRate = appointmentsResponse.Item3
                };
                return Ok(formattedResponse);
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
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> NumberAppointmentByStatus([FromRoute] Guid id, [FromQuery] DateTime? startdate, [FromQuery] DateTime enddate)
        {
            try
            {
                var appointmentsResponse = await _appointmentService.NumberofAppointmentByStatus(id, startdate, enddate);
                var formattedResponse = appointmentsResponse.Select(a => new
                {
                    Date = a.Item1,
                    Successed = a.Item2,
                    Failed = a.Item3,
                    CancelByCustomer = a.Item4
                });
                return Ok(formattedResponse);
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
