using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.BusySchedule;
using Hairhub.Domain.Dtos.Responses.BusySchedule;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.BusyScheduleEmployee.BusyScheduleEmployeeEndpoint + "/[action]")]
    [ApiController]
    public class BusyScheduleEmployeeController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IBusyScheduleEmployeeSerivce _busyScheduleEmployeeSerivce;

        public BusyScheduleEmployeeController(IMapper mapper, IBusyScheduleEmployeeSerivce busyScheduleEmployeeSerivce) : base(mapper)
        {
            this.mapper = mapper;
            _busyScheduleEmployeeSerivce = busyScheduleEmployeeSerivce;
        }

        [HttpPost]
        [Route("{id:Guid}")]
        
        public async Task<IActionResult> BusySchedule([FromRoute] Guid id, [FromBody] RequestCreationOfBusySchedule request)
        {
            try
            {
                var busyschedule = await _busyScheduleEmployeeSerivce.CreationofaBusySchedule(id, request);
                return Ok(busyschedule);               
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

        public async Task<IActionResult> DeleteBusySchedule([FromRoute] Guid id) { 
            try
            {
                var busyschedule = await _busyScheduleEmployeeSerivce.DeleteofaBusySchedule(id);
                return Ok(busyschedule);
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

        public async Task<IActionResult> UpdateBusySchedule([FromRoute] Guid id, [FromBody] RequestCreationOfBusySchedule request)
        {
            try
            {
                var busyschedule = await _busyScheduleEmployeeSerivce.UpdateofaBusySchedule(id, request);
                return Ok(busyschedule);
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
