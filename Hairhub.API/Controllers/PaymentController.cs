using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Payment.PaymentEndpoint+ "/[action]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentservice;

        public PaymentController(IMapper mapper, IPaymentService paymentservice) : base(mapper)
        {
            _paymentservice = paymentservice;
        }


        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> SendPaymentLink(CreatePaymentRequest request)
        {
            var result = await _paymentservice.CreatePaymentUrlRegisterCreator(request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateFirstTimePayment(CreateFirstTimePaymentRequest createFirstTimePaymentRequest)
        {
            var result = await _paymentservice.CreateFirstTimePayment(createFirstTimePaymentRequest);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateFirstTimePaymentCommissionRate(SavePaymentInfor createFirstTimePaymentRequest)
        {
            var result = await _paymentservice.PaymentForCommissionRate(createFirstTimePaymentRequest);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetStatusPayment(string ordercode, [FromBody] SavePaymentInfor paymentrequest)
        {
            var result = await _paymentservice.GetPaymentInfo(ordercode, paymentrequest);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetPayments([FromQuery]string? email, int page=1, int size = 10)
        {
            var result = await _paymentservice.GetPayments(email, page, size);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetPaymentHistory([FromQuery] DateTime? payDate, [FromQuery] Guid? accountId, [FromQuery] string? email, 
                                                            [FromQuery] string? paymentType, [FromQuery] string? status, 
                                                            [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var result = await _paymentservice.GetPaymentHistory(payDate, accountId, email, paymentType, status, page, size);
                return Ok(result);
            }
            catch (NotFoundException ex) 
            {
                return NotFound(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{ownerId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetPaymentByOwnerId([FromRoute] Guid ownerId, int page=1, int size=10)
        {
            var result = await _paymentservice.GetPaymentBySalonOwnerID(ownerId, page, size);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("{ownerId:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> GetInformationPaymentOfSalonOwner([FromRoute] Guid ownerId)
        {
            try
            {
                var result = await _paymentservice.GetInformationPaymentOfSalon(ownerId);
                if (result == null)
                {
                    return BadRequest(new { message = "Thất bại trong việc thanh toán" });
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
    }
}
