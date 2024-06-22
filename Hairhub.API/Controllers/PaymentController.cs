using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Service.Services.IServices;
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
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetListPayment([FromRoute]Guid id)
        {
            var result = await _paymentservice.GetPaymentBySalonOwnerID(id);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments(int page, int size)
        {
            var result = await _paymentservice.GetPayments(page, size);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
