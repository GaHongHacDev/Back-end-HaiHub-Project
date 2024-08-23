using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Customers;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Customer.CustomersEndpoint + "/[action]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(IMapper mapper, ICustomerService customerService) : base(mapper)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetAllCustomer([FromQuery] int page=1, [FromQuery] int size=10) {
            try
            {
                var customers = await _customerService.GetCustomers(page, size);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> CheckInByCustomer(CheckInRequest checkInRequest)
        {
            try
            {
                bool isCheckIn = await _customerService.CheckInByCustomer(checkInRequest.DataString, checkInRequest.CustomerId);
                if (!isCheckIn)
                {
                    return BadRequest(new { message = "Checkin thất bại. Vui lòng thử lại" });
                }
                return Ok("Checkin thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            try
            {
                var Customer = await _customerService.GetCustomerById(id);
                return Ok(Customer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> SaveAsCustomerImageHistory([FromForm]CustomerImageHistoryRequest request)
        {
            try
            {
                var result = await _customerService.SaveAsCustomerImageHistory(request);
                if (result == null)
                {
                    return BadRequest(new { message = "Thất bại trong việc tạo lịch sử" });
                }
                return Ok(new { success = result, message = "Tạo thành công" });
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
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetCustomerImageHistoryByCustomerId([FromRoute]Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var result = await _customerService.GetCustomerImagesHistory(id, page, size);
                if (result == null)
                {
                    return BadRequest(new { message = "Thất bại trong việc lấy lịch sử"});
                }
                return Ok(result);
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
        public async Task<IActionResult> UpdateCustomerImageHistory([FromRoute] Guid id, [FromForm] UpdateCustomerImageHistoryRequest request)
        {
            try
            {
                var result = await _customerService.UpdateCustomerImagesHistory(id, request);
                if (result == null)
                {
                    return BadRequest(new { message = "Thất bại trong việc lấy lịch sử" });
                }
                return Ok(new { success = result, message = "Cập nhập  thành công" });
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

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> DeleteCustomerImageHistory([FromRoute] Guid id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerImageHistory(id);
                if (result == null)
                {
                    return BadRequest(new { message = "Thất bại trong việc xóa lịch sử" });
                }
                return Ok(new { success = result, message = "Xóa thành công" });
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
