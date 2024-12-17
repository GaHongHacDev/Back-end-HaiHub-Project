using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using static QRCoder.PayloadGenerator;

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
                string code = Request.Query["code"]!;
                string des = Request.Query["desc"]!;
                string accountid = Request.Query["accountId"]!;
                string configid = Request.Query["config"]!;
                string appointment = Request.Query["appointment"]!;
                string salon = Request.Query["salon"]!;
                var price = Decimal.Parse(Request.Query["amount"]!)!;
                int orderCode = int.Parse(Request.Query["ordercode"]!);


                var request = new QueryRequest
                {
                    accountid = accountid,
                    Code = code,
                    configid = configid,
                    appontmentid = appointment,
                    salonid = salon,
                    des = des,
                    orderCode = orderCode,
                    Paymentlink = paymentlink,
                    price = price,
                    Status = status,
                };
                if (string.IsNullOrEmpty(paymentlink) || string.IsNullOrEmpty(status))
                {
                    return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");
                }

                var result = await _paymentservice.ConfirmPayment(Request.QueryString.Value!, request);
                string formattedAmount = $"{request.price:N0} VND";
                

                if (result != null && request.Code == "00")
                {
                    // Thanh toán thành công
                    var successHtml = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Payment Success</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; }}
                        .container {{ margin-top: 50px; }}
                        h1 {{ color: #4CAF50; font-size: 36px; font-weight: bold; }}
                        p {{ font-size: 18px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>THÀNH CÔNG RÙI NÈ</h1>
                        <p>Mã giao dịch: {request.Code}</p>
                        <p>Số tiền: {formattedAmount}</p>
                        <p>Cảm ơn bạn đã thanh toán!</p>
                    </div>
                </body>
                </html>";
                    return Content(successHtml, "text/html");
                }
                else
                {
                    // Thanh toán thất bại
                    var failureHtml = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Payment Failed</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; }}
                        .container {{ margin-top: 50px; }}
                        h1 {{ color: #FF0000; font-size: 36px; font-weight: bold; }}
                        p {{ font-size: 18px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>THẤT BẠI RÙI NÈ</h1>
                        <p>Mã giao dịch: {request.Code}</p>
                        <p>Số tiền: {formattedAmount}</p>
                        <p>Xin vui lòng thử lại hoặc liên hệ hỗ trợ.</p>
                    </div>
                </body>
                </html>";
                    return Content(failureHtml, "text/html");
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
        public async Task<IActionResult> CreateWithdrawPayment([FromForm] CreateWithdrawPaymentRequest request)
        {
            try
            {
                var result = await _paymentservice.CreateWithdrawPayment(request);

                if (result == null)
                {
                    return BadRequest("Lỗi không thể tạo đơn rút tiền");
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

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetPaymentReport([FromQuery] Guid? accountId, [FromQuery] string? email, [FromQuery] DateTime? createDate,
                                                    [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var result = await _paymentservice.GetPaymentReport(accountId, email, createDate, status, page, size);
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

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetPaymentReportById([FromRoute] Guid id)
        {
            try
            {
                var result = await _paymentservice.GetPaymentReportById(id);
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> GetPaymentCommistionInformation([FromBody] PaymentCommissionInforRequest request)
        {
            try
            {
                var result = await _paymentservice.GetCommissionOfPayment(request);
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
