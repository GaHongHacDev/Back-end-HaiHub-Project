using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Payment.PaymentEndpoint + "/[action]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentservice;

        public PaymentController(IMapper mapper, IPaymentService paymentservice) : base(mapper)
        {
            _paymentservice = paymentservice;
        }


        [HttpPost]       
        [Route("{accountid:Guid}")]
        public async Task<IActionResult> SendPaymentLink(Guid accountid, CreatePaymentRequest request)
        {
            var result = await _paymentservice.SendPaymentLink(accountid, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentConfirm()
        {
            // Kiểm tra xem có query string không
            if (Request.Query.Count > 0)
            {
                string paymentlink = Request.Query["id"]!;
                string status = Request.Query["status"]!;
                var accountid = Guid.Parse(Request.Query["accountId"]!);
                var configid = Guid.Parse(Request.Query["config"]!);
                var price = Decimal.Parse(Request.Query["amount"]!)!;
                int orderCode = int.Parse(Request.Query["ordercode"]!);


                if (string.IsNullOrEmpty(paymentlink) || string.IsNullOrEmpty(status))
                {
                    return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");
                }


                
                bool isValid = await _paymentservice.ConfirmPayment(Request.QueryString.Value!, paymentlink, accountid, (decimal)price, configid);


                if (isValid != true)
                {
                    // Thanh toán thành công
                    return Ok(isValid);
                }
                else
                {
                    // Thanh toán không thành công
                    return Ok(isValid);
                }

            }
            return Ok(false);
        }
        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateFirstTimePaymentCommissionRate(SavePaymentInfor createFirstTimePaymentRequest)
        {
            var result = await _paymentservice.FakePaymentForCommissionRate(createFirstTimePaymentRequest);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreatePromotionPaymentCommissionRate(SavePaymentInfor createFirstTimePaymentRequest)
        {
            var result = await _paymentservice.PromotionPaymentForCommissionRate(createFirstTimePaymentRequest);
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
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleNameAuthor.Customer + "," + RoleNameAuthor.SalonOwner)]
        public async Task<IActionResult> CreateWithdrawPayment(CreateWithdrawPaymentRequest request)
        {
            var result = await _paymentservice.CreateWithdrawPayment(request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> ConfirmWithdrawPayment([FromRoute] Guid id, [FromForm] WithdrawConfirmRequest request)
        {
            try
            {
                var result = await _paymentservice.ConfirmWithdrawPayment(id, request);
                if (result == null || !result)
                {
                    return BadRequest(new { message = "Lỗi không thể confirm đơn rút tiền" });
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
