using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Feedbacks;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Feedback.FeedbacksEndpoint + "/[action]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService _feedbackService)
        {
            this._feedbackService = _feedbackService;
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetFeedbacks(int page=1, int size = 10)
        {
            try
            {
                var schedules = await _feedbackService.GetFeedbacks(page, size);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetFeedbackById(Guid id)
        {
            try
            {
                var schedule = await _feedbackService.GetFeedbackById(id);
                if (schedule == null)
                {
                    return NotFound();
                }
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> CreateFeedback([FromForm]CreateFeedbackRequest request)
        {
            try
            {
                var isSuccessFull = await _feedbackService.CreateFeedback(request);
                if (!isSuccessFull)
                {
                    return BadRequest(new { message = "Lỗi trong quá trình tạo đánh giá dịch vụ salon, barber shop" });
                }
                return Ok("Tạo đánh giá thành công");
            }
            catch(NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new{message = ex.Message});
            }
        }

        [HttpPatch]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Customer)]
        public async Task<IActionResult> UpdateFeedback(Guid id, UpdateFeedbackRequest request)
        {
            try
            {
                var isSuccessfull = await _feedbackService.UpdateFeedback(id, request);
                if (!isSuccessfull)
                {
                    return BadRequest(new { message = "Không thể cập nhật đánh giá"});
                }
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            try
            {
                var isSuccessfull = await _feedbackService.DeleteFeedback(id);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetFeedBackBySalonId([FromRoute]Guid id, [FromQuery] decimal? rating, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var isSuccessfull = await _feedbackService.GetFeedBackBySalonId(id, rating, page, size);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetFeedBackByAppointmentId([FromRoute] Guid id)
        {
            try
            {
                var isSuccessfull = await _feedbackService.GetFeedBackByAppointmentId(id);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetFeedBackByCustomerId([FromRoute] Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var isSuccessfull = await _feedbackService.GetFeedbackByCustomerId(id, page, size);
                return Ok(isSuccessfull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
