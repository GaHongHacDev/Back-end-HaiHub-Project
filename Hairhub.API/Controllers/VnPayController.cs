using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.VnPay;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.VnPay.VnPayEndpoint + "/[action]")]
    [ApiController]
    public class VnPayController : BaseController
    {
        private readonly IVNPayService _vnpayservice;
        private readonly HttpClient _httpClient;

        public VnPayController(IVNPayService vnpayservice, IMapper mapper, HttpClient httpClient) : base(mapper)
        {
            _vnpayservice = vnpayservice;
            _httpClient = httpClient;
        }

        //[HttpPost]
        //public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        //{
        //    var url = _vnpayservice.CreatePaymentUrl(model, HttpContext);
        //    return Ok(url);  // Sửa lỗi OK bằng Ok()
        //}

        //[HttpGet]
        //public IActionResult PaymentCallback()
        //{
        //    var response = _vnpayservice.PaymentExecute(Request.Query);
        //    return new JsonResult(response);  // Sửa lỗi Json bằng JsonResult
        //}
        
        [HttpPost]
        //[Route("/VNPayAPI/{amount}&{infor}&{orderinfor}")]
        public ActionResult Payment(double amount, string infor, string orderinfor)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            string returnUrl = $"https://localhost:7257/api/v1/VnPays/PaymentConfirm";

            string paymentUrl = _vnpayservice.CreatePaymentUrl(amount, infor, orderinfor, returnUrl, clientIPAddress);
            return Ok(paymentUrl);
        }
        [HttpGet]
        
        public IActionResult PaymentConfirm()
        {
            // Kiểm tra xem có query string không
            if (Request.Query.Count > 0)
            {
                // Lấy các giá trị từ query string
                string orderInfor = Request.Query["vnp_OrderInfo"];
                string responseCode = Request.Query["vnp_ResponseCode"];

                // Kiểm tra tính hợp lệ của các giá trị nhận được
                if (string.IsNullOrEmpty(orderInfor) || string.IsNullOrEmpty(responseCode))
                {
                    return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE"); // Nếu không có thông tin cần thiết
                }

                // Xác nhận thanh toán với dịch vụ VNPAY
                bool isValid = _vnpayservice.ConfirmPayment(Request.QueryString.Value, out orderInfor, out responseCode);

                if (isValid && responseCode == "00")
                {
                    // Thanh toán thành công
                    return Redirect("LINK_THANH_CONG");
                }
                else
                {
                    // Thanh toán không thành công
                    return Redirect("LINK_KHONG_THANH_CONG");
                }
            }

            // Phản hồi không hợp lệ
            return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");
        }
        [HttpPost]
        public async Task<bool> CheckAccountExists(string accountNumber, string bankName)
        {
            var url = $"https://api.bank.com/checkAccount?accountNumber={accountNumber}&bankName={bankName}";
            Console.WriteLine($"Accessing URL: {url}"); // Log the URL
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<bool>(jsonString);
            }
            return false;
        }


    }
}
