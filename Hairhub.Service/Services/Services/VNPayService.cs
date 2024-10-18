using AutoMapper;
using Hairhub.Domain.Dtos.Requests.VnPay;
using Hairhub.Domain.Dtos.Responses.VnPay;
using Hairhub.Domain.Entitities;
using Hairhub.Service.Helpers;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace Hairhub.Service.Services.Services
{
    public class VNPayService : IVNPayService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        

        public VNPayService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }        
        //public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        //{
        //    var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_config["TimeZoneId"]);
        //    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        //    var tick = DateTime.Now.Ticks.ToString();
        //    var pay = new VnPayLibrary();
        //    var urlCallBack = _config["PaymentCallBack:ReturnUrl"];

        //    pay.AddRequestData("vnp_Version", _config["Vnpay:Version"]);
        //    pay.AddRequestData("vnp_Command", _config["Vnpay:Command"]);
        //    pay.AddRequestData("vnp_TmnCode", _config["Vnpay:vnp_TmnCode"]);
        //    pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
        //    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        //    pay.AddRequestData("vnp_CurrCode", _config["Vnpay:CurrCode"]);
        //    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        //    pay.AddRequestData("vnp_Locale", _config["Vnpay:Locale"]);
        //    pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
        //    pay.AddRequestData("vnp_OrderType", model.OrderType);
        //    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
        //    pay.AddRequestData("vnp_TxnRef", tick);

        //    var paymentUrl =
        //        pay.CreateRequestUrl(_config["Vnpay:vnp_Url"], _config["Vnpay:vnp_HashSecret"]);

        //    return paymentUrl;
        //}

        //public Task<string> Pay()
        //{
        //    //Get Config Info
        //    string vnp_Returnurl = _config["VNPay:vnp_Returnurl"];
        //    string vnp_Url = _config["VNPay:vnp_Url"];
        //    string vnp_TmnCode = _config["VNPay:vnp_TmnCode"];
        //    string vnp_HashSecret = _config["VNPay:vnp_HashSecret"];
        //    if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
        //    {
        //        throw new InvalidOperationException("Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file cấu hình.");

        //    }
        //    //Get payment input
        //    Appointment appointment = new Appointment();
        //    //Save order to db
        //    appointment.Id = Guid.NewGuid(); // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
        //    appointment.TotalPrice = 100000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
        //    appointment.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
        //    appointment.CreatedDate = DateTime.Now;


        //    VnPayLibrary vnpay = new VnPayLibrary();

            


        //    throw new NotImplementedException();
        //}

        //public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        //{
        //    var pay = new VnPayLibrary();
        //    var response = pay.GetFullResponseData(collections, _config["Vnpay:HashSecret"]);

        //    return response;
        //}


        public string CreatePaymentUrl(double amount, string infor, string orderinfor, string returnUrl, string clientIPAddress)
        {
            VnPayLibrary pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", "2.1.0"); // Phiên bản API
            pay.AddRequestData("vnp_Command", "pay"); // Lệnh thanh toán
            pay.AddRequestData("vnp_TmnCode", _config["Vnpay:vnp_TmnCode"]); // Mã website của merchant
            pay.AddRequestData("vnp_Amount", ((int)amount * 100).ToString()); // Số tiền cần thanh toán
            pay.AddRequestData("vnp_BankCode", ""); // Mã Ngân hàng thanh toán (nếu có)
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); // Ngày thanh toán
            pay.AddRequestData("vnp_CurrCode", "VND"); // Đơn vị tiền tệ
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); // Địa chỉ IP của khách hàng
            pay.AddRequestData("vnp_Locale", "vn"); // Ngôn ngữ giao diện
            pay.AddRequestData("vnp_OrderInfo", infor); // Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); // Loại đơn hàng
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); // URL trả kết quả giao dịch
            pay.AddRequestData("vnp_TxnRef", orderinfor); // Mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(_config["Vnpay:vnp_Url"], _config["Vnpay:vnp_HashSecret"]);
            return paymentUrl;
        }

        public bool ConfirmPayment(string queryString, out string orderInfor, out string responseCode)
        {
            var json = HttpUtility.ParseQueryString(queryString);
            long orderId = Convert.ToInt64(json["vnp_TxnRef"]); // Mã hóa đơn
            orderInfor = json["vnp_OrderInfo"]; // Thông tin giao dịch
            long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); // Mã giao dịch
            responseCode = json["vnp_ResponseCode"]; // Mã phản hồi
            string vnp_SecureHash = json["vnp_SecureHash"]; // Hash của dữ liệu trả về
            var pos = queryString.IndexOf("&vnp_SecureHash");

            bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash);
            return checkSignature && _config["Vnpay:vnp_TmnCode"] == json["vnp_TmnCode"];
        }

        private bool ValidateSignature(string rspraw, string inputHash)
        {
            string myChecksum = VnPayLibrary.HmacSHA512(_config["Vnpay:vnp_HashSecret"], rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
