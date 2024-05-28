﻿using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
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
        public async Task<IActionResult> GetAllSalonInformation([FromQuery] int page, [FromQuery] int size)
        {
            var salonInformationsResponse = await _salonInformationService.GetAllSalonInformation(page, size);
            return Ok(salonInformationsResponse);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetSalonInformationById([FromRoute] Guid id)
        {
            try
            {
                var salonInformationResponse = await _salonInformationService.GetSalonInformationById(id);
                if (salonInformationResponse == null)
                {
                    return NotFound("Cannot find this SalonInformation!");
                }
                return Ok(salonInformationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalonInformation([FromBody] CreateSalonInformationRequest createSalonInformationRequest)
        {
            try
            {
                var accoutResponse = await _salonInformationService.CreateSalonInformation(createSalonInformationRequest);
                if (accoutResponse == null)
                {
                    return BadRequest("Cannot create SalonInformation!");
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
        public async Task<IActionResult> UpdateSalonInformation([FromRoute] Guid id, [FromBody] UpdateSalonInformationRequest updateSalonInformationRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("SalonInformation Id is null or empty!");
                }

                bool isUpdate = await _salonInformationService.UpdateSalonInformationById(id, updateSalonInformationRequest);
                if (!isUpdate)
                {
                    return BadRequest("Cannot update SalonInformation");
                }
                return Ok("Update SalonInformation successfully");
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
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}