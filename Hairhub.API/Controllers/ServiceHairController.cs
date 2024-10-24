﻿using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.ServiceHair.ServiceHairsEndpoint + "/[action]")]
    [ApiController]
    public class ServiceHairController : BaseController
    {
        private readonly IServiceHairService _serviceHairService;

        public ServiceHairController(IMapper mapper, IServiceHairService serviceHairService) : base(mapper)
        {
            _serviceHairService = serviceHairService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceHair([FromQuery] int page=1, [FromQuery] int size=10)
        {
            var serviceHairsResponse = await _serviceHairService.GetAllServiceHair(page, size);
            return Ok(serviceHairsResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetServiceHairById([FromRoute] Guid id)
        {
            try
            {
                var serviceHairResponse = await _serviceHairService.GetServiceHairById(id);
                if (serviceHairResponse == null)
                {
                    return NotFound("Cannot find this ServiceHair!");
                }
                return Ok(serviceHairResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{salonInformationId:Guid}")]
        public async Task<IActionResult> GetServiceHairBySalonInformationId([FromRoute]Guid salonInformationId)
        {
            try
            {
                var services = await _serviceHairService.GetServiceHairBySalonInformationId(salonInformationId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateServiceHair([FromForm] CreateServiceHairRequest createServiceHairRequest)
        {
            try
            {
                var accoutResponse = await _serviceHairService.CreateServiceHair(createServiceHairRequest);
                if (!accoutResponse)
                {
                    return BadRequest("Cannot create ServiceHair!");
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
        public async Task<IActionResult> UpdateServiceHair([FromRoute] Guid id, [FromForm] UpdateServiceHairRequest updateServiceHairRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("ServiceHair Id is null or empty!");
                }

                bool isUpdate = await _serviceHairService.UpdateServiceHairById(id, updateServiceHairRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update ServiceHair");
                }
                return Ok("Update ServiceHair successfully");
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
        public async Task<IActionResult> DeleteServiceHair([FromRoute] Guid id)
        {
            {
                try
                {
                    var isDelete = await _serviceHairService.DeleteServiceHairById(id);
                    if (!isDelete)
                    {
                        return BadRequest("Cannot delete this ServiceHair!");
                    }
                    return Ok("Delete ServiceHair successfully!");
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
        public async Task<IActionResult> ActiveServiceHair([FromRoute] Guid id)
        {
            try
            {
                var isActive = await _serviceHairService.ActiveServiceHair(id);
                if (!isActive)
                {
                    return BadRequest("Cannot delete this ServiceHair!");
                }
                return Ok("ServiceHair account successfully!");
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
