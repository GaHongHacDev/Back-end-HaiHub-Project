using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.SalonOwners;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.SalonOwner.SalonOwnersEndpoint + "/[action]")]
    [ApiController]
    public class SalonOwnerController : BaseController
    {
        private readonly ISalonOwnerService _salonOwnerService;

        public SalonOwnerController(IMapper mapper, ISalonOwnerService salonOwnerService) : base(mapper)
        {
            _salonOwnerService = salonOwnerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalonOwner([FromQuery] int page=1, [FromQuery] int size=10)
        {
            try
            {
                var salonOwnersResponse = await _salonOwnerService.GetAllSalonOwner(page, size);
                return Ok(salonOwnersResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetSalonOwnerById([FromRoute] Guid id)
        {
            try
            {
                var salonOwnerResponse = await _salonOwnerService.GetSalonOwnerById(id);
                if (salonOwnerResponse == null)
                {
                    return NotFound("Cannot find this SalonOwner!");
                }
                return Ok(salonOwnerResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalonOwner([FromBody] CreateSalonOwnerRequest createSalonOwnerRequest)
        {
            try
            {
                var accoutResponse = await _salonOwnerService.CreateSalonOwner(createSalonOwnerRequest);
                if (accoutResponse == null)
                {
                    return BadRequest("Cannot create SalonOwner!");
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
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateSalonOwner([FromRoute] Guid id, [FromBody] UpdateSalonOwnerRequest updateSalonOwnerRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("SalonOwner Id is null or empty!");
                }

                bool isUpdate = await _salonOwnerService.UpdateSalonOwnerById(id, updateSalonOwnerRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update SalonOwner");
                }
                return Ok("Update SalonOwner successfully");
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
        public async Task<IActionResult> DeleteSalonOwner([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _salonOwnerService.DeleteSalonOwnerById(id);
                    if (!isDelete)
                    {
                        return BadRequest("Cannot delete this SalonOwner!");
                    }
                    return Ok("Delete SalonOwner successfully!");
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
