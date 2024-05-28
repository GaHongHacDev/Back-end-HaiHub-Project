using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Schedule.SchedulesEndpoint + "/[action]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService _scheduleService)
        {
            this._scheduleService = _scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules(int page, int size) {
            try
            {
                var schedules = await _scheduleService.GetSchedules(page, size);
                return Ok(schedules);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetScheduleById(Guid id) {
            try
            {
                var schedule = await _scheduleService.GetScheduleById(id);
                if (schedule == null)
                {
                    return NotFound();
                }
                return Ok(schedule);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(CreateScheduleRequest request)
        {
            try
            {
                var isSuccessFull = await _scheduleService.CreateSchedue(request);
                return Ok(isSuccessFull);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateSchedule(Guid id, UpdateScheduleRequest request) { 
            try
            {
                var isSuccessfull = await _scheduleService.UpdateSchedule(id, request);
                return Ok(isSuccessfull);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            try
            {
                var isSuccessfull = await _scheduleService.DeleteSchedule(id);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
