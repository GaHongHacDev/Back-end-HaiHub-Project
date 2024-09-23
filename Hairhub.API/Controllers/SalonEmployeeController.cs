
using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.SalonEmployee.SalonEmployeesEndpoint + "/[action]")]
    [ApiController]
    public class SalonEmployeeController : BaseController
    {
        private readonly ISalonEmployeeService _salonEmployeeService;

        public SalonEmployeeController(IMapper mapper, ISalonEmployeeService salonEmployeeService) : base(mapper)
        {
            _salonEmployeeService = salonEmployeeService;
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetAllSalonEmployee([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var salonEmployeesResponse = await _salonEmployeeService.GetAllSalonEmployee(page, size);
            return Ok(salonEmployeesResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetSalonEmployeeById([FromRoute] Guid id)
        {
            try
            {
                var salonEmployeeResponse = await _salonEmployeeService.GetSalonEmployeeById(id);
                if (salonEmployeeResponse == null)
                {
                    return NotFound("Cannot find this SalonEmployee!");
                }
                return Ok(salonEmployeeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{SalonInformationId:Guid}")]
        public async Task<IActionResult> GetSalonEmployeeBySalonInformationId([FromRoute] Guid SalonInformationId, [FromQuery] int page = 1, [FromQuery] int size = 10,
                                                                              [FromQuery] bool? orderByName = null, [FromQuery] bool? isActive = null, [FromQuery] string? nameEmployee = null)
        {
            try
            {
                var salonEmployeeResponse = await _salonEmployeeService.GetSalonEmployeeBySalonInformationId(SalonInformationId, page, size, orderByName, isActive, nameEmployee);
                if (salonEmployeeResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy dịch vụ" });
                }
                return Ok(salonEmployeeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateSalonEmployee([FromForm] CreateSalonEmployeeRequest createSalonEmployeeRequest)
        {
            try
            {
                var accoutResponse = await _salonEmployeeService.CreateSalonEmployee(createSalonEmployeeRequest);
                if (!accoutResponse)
                {
                    return BadRequest("Cannot create SalonEmployee!");
                }
                return Ok(accoutResponse);
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

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> UpdateSalonEmployee([FromRoute] Guid id, [FromForm] UpdateSalonEmployeeRequest updateSalonEmployeeRequest)
        {
            try
            {
                bool isUpdate = await _salonEmployeeService.UpdateSalonEmployeeById(id, updateSalonEmployeeRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update SalonEmployee");
                }
                return Ok("Update SalonEmployee successfully");
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

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> DeleteSalonEmployee([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _salonEmployeeService.DeleteSalonEmployeeById(id);
                    if (!isDelete)
                    {
                        return BadRequest("Không thể xóa nhân viên này!");
                    }
                    return Ok("Xóa nhân viên thành công!");
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

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> ActiveSalonEmployee([FromRoute] Guid id)
        {
            try
            {
                var isActive = await _salonEmployeeService.ActiveSalonEmployee(id);
                if (!isActive)
                {
                    return BadRequest("Cannot delete this SalonEmployee!");
                }
                return Ok("SalonEmployee account successfully!");
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateAccountEmployee([FromBody] CreateAccountEmployeeRequest createAccountEmployeeRequest)
        {
            try
            {
                var response = await _salonEmployeeService.CreateAccountEmployee(createAccountEmployeeRequest);
                return Ok(createAccountEmployeeRequest);
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
        [Route("{numberOfDay}")]
        public async Task<IActionResult> GetEmployeesHighRating([FromRoute] int? numberOfDay)
        {
            try
            {
                var result = await _salonEmployeeService.GetEmployeeHighRating(numberOfDay);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
