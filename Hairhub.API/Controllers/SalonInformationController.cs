using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.SalonInformation.SalonInformationsEndpoint + "/[action]")]
    [ApiController]
    public class SalonInformationController : BaseController
    {
        private readonly ISalonInformationService _salonInformationService;

        public SalonInformationController(IMapper mapper, ISalonInformationService salonInformationService) : base(mapper)
        {
            _salonInformationService = salonInformationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalonInformation([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var salonInformationsResponse = await _salonInformationService.GetAllApprovedSalonInformation(page, size);
            return Ok(salonInformationsResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalonInformationNoPaging()
        {
            var salonInformationsResponse = await _salonInformationService.GetAllApprovedSalonInformationNoPaging();
            return Ok(salonInformationsResponse);
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetAllSalonByAdmin([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var salonInformationsResponse = await _salonInformationService.GetAllSalonByAdmin(page, size);
            return Ok(salonInformationsResponse);
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetSalonByStatus([FromQuery] string? name, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var salonInformationsResponse = await _salonInformationService.GetSalonByStatus(name, status, page, size);
            return Ok(salonInformationsResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalonSuggestion()
        {
            var salonInformationsResponses = await _salonInformationService.GetSalonSuggestion();
            return Ok(salonInformationsResponses);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetSalonInformationById([FromRoute] Guid id)
        {
            try
            {
                var salonInformationResponse = await _salonInformationService.GetSalonInformationById(id)!;
                if (salonInformationResponse == null)
                {
                    return NotFound("Cannot find this SalonInformation!");
                }
                return Ok(salonInformationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> GetSalonByEmployeeId([FromRoute] Guid id)
        {
            try
            {
                var salonInformationResponse = await _salonInformationService.GetSalonByEmployeeId(id)!;
                if (salonInformationResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy Salon, barber shop" });
                }
                return Ok(salonInformationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{ownerId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetSalonInformationByOwnerId([FromRoute] Guid ownerId)
        {
            try
            {
                var salonInformationResponse = await _salonInformationService.GetSalonByOwnerId(ownerId)!;
                return Ok(salonInformationResponse);
            }
            catch (NotFoundException nf)
            {
                return NotFound(new { message = nf.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalonByServiceNameAddress(
            [FromQuery] string? serviceName = null,
            [FromQuery] string? salonAddress = null,
            [FromQuery] string? salonName = null,
            [FromQuery] decimal? latitude = null,
            [FromQuery] decimal? longtitude = null,
            [FromQuery] decimal? distance = null,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            try
            {
                var salonInformationResponse = await _salonInformationService
                                                        .SearchSalonByNameAddressService(
                                                                                         page, size, serviceName, salonAddress, salonName,
                                                                                         latitude, longtitude, distance
                                                                                         );
                return Ok(salonInformationResponse);
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
        public async Task<IActionResult> CreateSalonInformation([FromForm] CreateSalonInformationRequest createSalonInformationRequest)
        {
            try
            {
                var accoutResponse = await _salonInformationService.CreateSalonInformation(createSalonInformationRequest);
                if (accoutResponse == null)
                {
                    return NotFound("Không thể tạo Salon, barber shop");
                }
                return Ok(accoutResponse);
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> UpdateSalonInformation([FromRoute] Guid id, [FromForm] UpdateSalonInformationRequest updateSalonInformationRequest)
        {
            try
            {
                bool isUpdate = await _salonInformationService.UpdateSalonInformationById(id, updateSalonInformationRequest);
                if (!isUpdate)
                {
                    return BadRequest("Không thể cập nhật thông tin Salon, Barber shop");
                }
                return Ok("Cập nhật thôn tin Salon, Barber shop thành công");
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> DeleteSalonInformation([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _salonInformationService.DeleteSalonInformationById(id);
                    if (!isDelete)
                    {
                        return BadRequest("Cannot delete this SalonInformation!");
                    }
                    return Ok("Delete SalonInformation successfully!");
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

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> ActiveSalonInformation([FromRoute] Guid id)
        {
            try
            {
                var isActive = await _salonInformationService.ActiveSalonInformation(id);
                if (!isActive)
                {
                    return BadRequest("Cannot delete this SalonInformation!");
                }
                return Ok("SalonInformation account successfully!");
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
        //[Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> ReviewRevenue([FromRoute] Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            try
            {
                var result = await _salonInformationService.ReviewRevenue(id, startTime, endTime);
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

        [HttpPost]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> AddSalonInformationImages([FromRoute]Guid id, [FromForm] AddSalonImagesRequest request)
        {
            try
            {
                bool isSuccessed = await _salonInformationService.AddSalonInformationImages(id, request);
                if (isSuccessed == false)
                {
                    return NotFound("Không thể thêm hình Salon, barber shop");
                }
                return Ok(isSuccessed);
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
        //[Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetSalonInformationImages([FromRoute] Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var SalonImages = await _salonInformationService.GetSalonInformationImages(id,page, size);
                if (SalonImages == null)
                {
                    return BadRequest("Không thể lấy hình của salon");
                }
                return Ok(SalonImages);
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
        public async Task<IActionResult> DeleteSalonInformationImages([FromBody] DeleteImagesRequest request)
        {
            try
            {
                bool isSuccessed = await _salonInformationService.DeleteSalonInformationImages(request);
                if (isSuccessed == false)
                {
                    return NotFound("Không thể xóa hình Salon, barber shop");
                }
                return Ok(isSuccessed);
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
