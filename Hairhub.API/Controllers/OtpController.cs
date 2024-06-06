using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Otp.OtpsEndpoint + "/[action]")]
    [ApiController]
    public class OtpController : BaseController
    {
        private readonly IEmailService _emailService;

        public OtpController(IMapper mapper, IEmailService emailService) : base(mapper)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendOTPToEmail([FromBody] SendOtpEmailRequest sendOtpEmailRequest)
        {
            try
            {
                bool isSendOtp = await _emailService.SendEmailAsync(sendOtpEmailRequest);
                if (!isSendOtp)
                {
                    return BadRequest("Cannot send mail!");
                }
                return Ok("Send Otp successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> checkOtp([FromBody] CheckOtpRequest CheckOtpRequest)
        {
            try
            {
                bool isValidOtp = await _emailService.CheckOtpEmail(CheckOtpRequest);
                if (!isValidOtp)
                {
                    return BadRequest("Otp is invalid!");
                }
                return Ok("Otp is valid!");
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

        [HttpPost]
        public async Task<IActionResult> CheckExistEmail([FromBody] CheckExistEmailResrequest request)
        {
            try
            {
                bool isExistEmail = await _emailService.CheckExistEmail(request);
                if (!isExistEmail)
                {
                    return NotFound("Email không tồn tại!");
                }
                return Ok("Email đã tồn tại!");
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
