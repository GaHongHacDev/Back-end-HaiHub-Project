using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Dtos.Requests.VnPay;
using Hairhub.Domain.Dtos.Responses.VnPay;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
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
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace Hairhub.Service.Services.Services
{
    public class VNPayService : IVNPayService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAppointmentService _appointmentservice;
        private readonly IPaymentService _paymentService;

        public VNPayService(IUnitOfWork unitOfWork, 
                            IMapper mapper, 
                            IConfiguration config, 
                            IAppointmentService appointmentService,
                            IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            _appointmentservice = appointmentService;
            _paymentService = paymentService;
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


        public async Task<string> CreatePaymentUrl(VnPayRequest request, string returnUrl, string clientIPAddress)
        {
            var Configs = await _unitOfWork.GetRepository<Domain.Entitities.Config>().SingleOrDefaultAsync(predicate: c => c.Id == request.ConfigId);
            var SalonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: s => s.Id == request.SalonOwnerId);
            if (SalonOwner == null)
            {
                throw new Exception("SalonOwner is null.");
            }


            var Salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: s => s.SalonOwner.Id == request.SalonOwnerId);
            if (Salon == null)
            {
                throw new Exception("Salon is null.");
            }
            //int amount = (int)await AmountofCommissionRateInMonthBySalon(SalonOwner.Id, (decimal)Configs.CommissionRate);
            int amount = 30000;
            VnPayLibrary pay = new VnPayLibrary();
            string currentTimeString = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            //long orderCode = long.Parse(currentTimeString.Substring(currentTimeString.Length - 6));
            pay.AddRequestData("vnp_Version", "2.1.0"); // Phiên bản API
            pay.AddRequestData("vnp_Command", "pay"); // Lệnh thanh toán
            pay.AddRequestData("vnp_TmnCode", _config["Vnpay:vnp_TmnCode"]); // Mã website của merchant
            pay.AddRequestData("vnp_Amount", ((int)amount * 100).ToString()); // Số tiền cần thanh toán
            pay.AddRequestData("vnp_BankCode", ""); // Mã Ngân hàng thanh toán (nếu có)
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); // Ngày thanh toán
            pay.AddRequestData("vnp_CurrCode", "VND"); // Đơn vị tiền tệ
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); // Địa chỉ IP của khách hàng
            pay.AddRequestData("vnp_Locale", "vn"); // Ngôn ngữ giao diện
            pay.AddRequestData("vnp_OrderInfo", $"{(string)request.ConfigId.ToString()} + {(string)request.SalonOwnerId.ToString()}"); // Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); // Loại đơn hàng
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); // URL trả kết quả giao dịch
            pay.AddRequestData("vnp_TxnRef", currentTimeString); // Mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(_config["Vnpay:vnp_Url"], _config["Vnpay:vnp_HashSecret"]);
            return paymentUrl;
        }

        public async Task<decimal> AmountofCommissionRateInMonthBySalon(Guid id, decimal commissionRate)
        {
            var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: p => p.SalonOwner.Id == id);
            if (salon == null)
            {
                return 0;
            }

            var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.SalonOWnerID == salon.OwnerId && p.Status == PaymentStatus.Fake);
            if (payment == null)
            {
                return 0;
            }

            var appointments = await _appointmentservice.GetAppointmentSalonByStatusNoPaing(salon.Id, AppointmentStatus.Successed, payment.StartDate, payment.EndDate);

            decimal totalCommission = 0;
            foreach (var appointment in appointments)
            {
                decimal commissionAmount = appointment.TotalPrice * (commissionRate / 100);
                totalCommission += commissionAmount;
            }

            return totalCommission;
        }

        public async Task<bool> ConfirmPayment(string queryString,  string orderInfor,  string responseCode)
        {
            var json = HttpUtility.ParseQueryString(queryString);   
            // Extracting values from query string
            long orderId = Convert.ToInt64(json["vnp_TxnRef"]); // Mã hóa đơn
            decimal amout = decimal.Parse(json["vnp_Amount"])/100;
            orderInfor = json["vnp_OrderInfo"]; // Thông tin giao dịch
            var parts = orderInfor.Split('+');
            var configId = Guid.Parse(parts[0].Trim());  // Parse ConfigId as Guid
            var salonOwnerId = Guid.Parse(parts[1].Trim());
            long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); // Mã giao dịch
            responseCode = json["vnp_ResponseCode"]; // Mã phản hồi
            string vnp_SecureHash = json["vnp_SecureHash"]; // Hash của dữ liệu trả về
            var pos = queryString.IndexOf("&vnp_SecureHash");
            bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash);
            if (!checkSignature)
            {
                return false;
            }
            var config = await _unitOfWork.GetRepository<Domain.Entitities.Config>().SingleOrDefaultAsync(predicate: p => p.Id == configId);

            if (config == null)
            {
                return false;
            }

            var paymentFake = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.SalonOWnerID == salonOwnerId && p.Status == PaymentStatus.Fake);

            if (responseCode == "00")
            {
                // Create a new payment record
                paymentFake.Status = PaymentStatus.Paid;
                paymentFake.MethodBanking = "VNPAY";
                paymentFake.TotalAmount = amout;
                _unitOfWork.GetRepository<Payment>().UpdateAsync(paymentFake);
                _unitOfWork.Commit();
                
            }
            return checkSignature && _config["Vnpay:vnp_TmnCode"] == json["vnp_TmnCode"];
        }

        private bool ValidateSignature(string rspraw, string inputHash)
        {
            string myChecksum = VnPayLibrary.HmacSHA512(_config["Vnpay:vnp_HashSecret"], rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        
    }
}
