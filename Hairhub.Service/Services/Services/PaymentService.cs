using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Helpers;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Net.payOS;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hairhub.Domain.Dtos.Responses.Payment;
using Newtonsoft.Json.Linq;
using Hairhub.Domain.Dtos.Responses.Voucher;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.NetworkInformation;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using AutoMapper;
using SalonOwner = Hairhub.Domain.Entitities.SalonOwner;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Microsoft.EntityFrameworkCore;
using Hairhub.Domain.Dtos.Responses.Customers;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Enums;

namespace Hairhub.Service.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;
        private readonly HttpClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentservice;

        public PaymentService(IOptions<PayOSSettings> settings, HttpClient client, IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper, IAppointmentService appointmentService)
        {
            _payOSSettings = settings.Value;
            _appointmentservice = appointmentService;
            _client = client;
            _unitOfWork = unitOfWork;
            _config = config;
            _mapper = mapper;
        }

        private string ComputeHmacSha256(string data, string checksumKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public async Task<CreatePaymentResult> CreatePaymentUrlRegisterCreator(CreatePaymentRequest request)
        {
            try
            {
                var Configs = await _unitOfWork.GetRepository<Domain.Entitities.Config>().SingleOrDefaultAsync(predicate: c => c.Id == request.ConfigId);
                var SalonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: s => s.Id == request.SalonOWnerID);
                var Salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: s => s.SalonOwner.Id == SalonOwner.Id);


                int amount = (int)await AmountofCommissionRateInMonthBySalon(Salon.Id, (decimal)Configs.CommissionRate);
                var orderCode = new Random().Next(1, 1000000);
                var description = request.Description;
                string? clientId = _config["PayOS:ClientId"];
                var apikey = _config["PayOS:APIKey"];
                var checksumkey = _config["PayOS:ChecksumKey"];
                var returnurl = _config["PayOS:ReturnUrl"];
                var returnurlfail = _config["PayOS:ReturnUrlFail"];

                var updatedReturnUrl = $"{returnurl}?orderCode={Uri.EscapeDataString(orderCode.ToString())}&configId={Uri.EscapeDataString(Configs.Id.ToString())}&amount={amount}";
                var updatedReturnUrlFail = $"{returnurlfail}?orderCode={Uri.EscapeDataString(orderCode.ToString())}&configId={Uri.EscapeDataString(Configs.Id.ToString())}&amount={amount}";

                PayOS pos = new PayOS(clientId, apikey, checksumkey);
                // Prepare data for signature
                var signatureData = new Dictionary<string, object>
                 {
                     { "amount", amount },
                     { "cancelUrl", updatedReturnUrlFail},
                     { "description", description },
                     { "expiredAt", DateTimeOffset.Now.ToUnixTimeSeconds() },
                     { "orderCode", orderCode },
                     { "returnUrl", updatedReturnUrl}
                 };



                // Sort data alphabetically by key
                var sortedSignatureData = new SortedDictionary<string, object>(signatureData);

                // Create data string for signature
                var dataForSignature = string.Join("&", sortedSignatureData.Select(p => $"{p.Key}={p.Value}"));

                // Compute the HMAC_SHA256 signature
                var signature = ComputeHmacSha256(dataForSignature, checksumkey);
                DateTimeOffset expiredAt = DateTimeOffset.Now.AddMinutes(10);

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    items: new List<ItemData>(), // Provide a list of items if needed
                    cancelUrl: updatedReturnUrlFail,
                    returnUrl: updatedReturnUrl,
                    signature: signature,
                    buyerName: SalonOwner.FullName,
                    buyerPhone: SalonOwner.Phone, // Provide a valid currency
                    buyerEmail: SalonOwner.Email,
                    buyerAddress: SalonOwner.Address,
                    expiredAt: (int)expiredAt.ToUnixTimeSeconds()
                );

                paymentData.items.Add(new ItemData(SalonOwner.FullName, 1, amount));
                var createPaymentResult = await pos.createPaymentLink(paymentData);

                return createPaymentResult; // Chú ý sử dụng PaymentLink thay vì paymentLink

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> GetPaymentInfo(string paymentLinkId, SavePaymentInfor createPaymentRequest)
        {
            var getUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{paymentLinkId}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, getUrl);
                request.Headers.Add("x-client-id", _config["PayOS:ClientId"]);
                request.Headers.Add("x-api-key", _config["PayOS:APIKey"]);
                var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: p => p.SalonOwner.Id == createPaymentRequest.SalonOwnerId);
                var response = await _client.SendAsync(request);
                bool isStatus = false;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JObject.Parse(responseContent);
                    var status = responseObject["data"]?["status"]?.ToString();
                    var paymentInfo = responseObject["data"];

                    if (status != null)
                    {
                        if (status == "PAID")
                        {
                            if (salon.Status == SalonStatus.Approved)
                            {
                                var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
                                var payment = _mapper.Map<Payment>(createPaymentRequest);
                                payment.Id = Guid.NewGuid();
                                payment.ConfigId = config.Id;
                                payment.SalonOWnerID = createPaymentRequest.SalonOwnerId;
                                payment.TotalAmount = (int)paymentInfo["amount"];
                                payment.PaymentCode = (int)paymentInfo["orderCode"];
                                payment.Description = "";
                                payment.PaymentDate = DateTime.Now;
                                payment.StartDate = DateTime.Now;
                                payment.EndDate = DateTime.Now.AddDays((double)config.NumberOfDay);
                                payment.MethodBanking = "PayOS";
                                payment.Status = PaymentStatus.Paid;
                                // Save the transaction
                                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                                await PaymentForCommissionRate(createPaymentRequest);
                                isStatus = await _unitOfWork.CommitAsync() > 0;
                                
                            } else if( salon.Status == SalonStatus.OverDue) {
                                var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
                                var payment = _mapper.Map<Payment>(createPaymentRequest);
                                payment.Id = Guid.NewGuid();
                                payment.ConfigId = config.Id;
                                payment.SalonOWnerID = createPaymentRequest.SalonOwnerId;
                                payment.TotalAmount = (int)paymentInfo["amount"];
                                payment.PaymentCode = (int)paymentInfo["orderCode"];
                                payment.Description = "";
                                payment.PaymentDate = DateTime.Now;
                                payment.StartDate = DateTime.Now;
                                payment.EndDate = DateTime.Now.AddDays((double)config.NumberOfDay);
                                payment.MethodBanking = "PayOS";
                                payment.Status = PaymentStatus.Paid;
                                salon.Status = SalonStatus.Approved;

                                // Save the transaction
                                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);
                                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                                await PaymentForCommissionRate(createPaymentRequest);
                                isStatus = await _unitOfWork.CommitAsync() > 0;
                                return isStatus;
                            }
                            

                        }
                        return isStatus;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve payment status.");
                    }
                }
                else
                {
                    throw new Exception("Failed to send request.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IPaginate<ResponsePayment>> GetPaymentBySalonOwnerID(Guid ownerid, int page, int size)
        {
            var existingsalonowner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: e => e.Id == ownerid);
            if (existingsalonowner == null)
            {
                throw new Exception("Not found");
            }
            var payments = await _unitOfWork.GetRepository<Payment>()
             .GetPagingListAsync(
                 include: query => query.Include(x => x.SalonOwner)
                         .Include(x => x.SalonOwner).ThenInclude(x => x.SalonInformations)
                         .Include(x => x.Config),
                 predicate: x => x.SalonOWnerID == ownerid && x.Status == PaymentStatus.Paid,
                 page: page,
                 size: size);

            var paginateResponse = new Paginate<ResponsePayment>
            {
                Page = payments.Page,
                Size = payments.Size,
                Total = payments.Total,
                TotalPages = payments.TotalPages,
                Items = _mapper.Map<IList<ResponsePayment>>(payments.Items)
            };

            return paginateResponse;
        }

        public async Task<IPaginate<ResponsePayment>> GetPayments(int page, int size)
        {

            var payments = await _unitOfWork.GetRepository<Payment>()
            .GetPagingListAsync(predicate: x => x.Status == PaymentStatus.Paid,
                include: query => query.Include(x => x.SalonOwner)
                                        .Include(x => x.SalonOwner).ThenInclude(x => x.SalonInformations)
                                        .Include(x => x.Config),
                page: page,
                size: size);

            var paginateResponse = new Paginate<ResponsePayment>
            {
                Page = payments.Page,
                Size = payments.Size,
                Total = payments.Total,
                TotalPages = payments.TotalPages,
                Items = _mapper.Map<IList<ResponsePayment>>(payments.Items)
            };

            return paginateResponse;
        }

        public async Task<bool> CreateFirstTimePayment(CreateFirstTimePaymentRequest createFirstTimePaymentRequest)
        {
           Guid salonownerid  = createFirstTimePaymentRequest.SalonOwnerId;
           var firstPayment = new Payment { 
               Id = Guid.NewGuid(),
               Description = "Miễn phí 1 tháng đầu tiên",
               StartDate = DateTime.Now,
               EndDate = DateTime.Now.AddDays(30),
               PaymentDate = DateTime.Now,
               SalonOWnerID = salonownerid,
               MethodBanking = "None",
               PaymentCode = new Random().Next(1, 1000000),
               Status = "PAID",
               TotalAmount = 0,
            };
            
            await _unitOfWork.GetRepository<Payment>().InsertAsync(firstPayment);
            bool isCreated = await _unitOfWork.CommitAsync() > 0;
            return isCreated;

        }

        public async Task<bool> PaymentForCommissionRate(SavePaymentInfor createPaymentRequest)
        {
            var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
            Guid salonownerid = createPaymentRequest.SalonOwnerId;
            var firstPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Description = "Tiền hoa hồng dựa trên lịch hẹn tháng đầu tiên",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PaymentDate = DateTime.Now,
                SalonOWnerID = salonownerid,
                MethodBanking = "None",
                PaymentCode = new Random().Next(1, 1000000),
                Status = PaymentStatus.Fake,
                TotalAmount = 0,
                ConfigId = config.Id,
                PakageFee = config.PakageFee,
                PakageName = config.PakageName,
                CommissionRate = config.CommissionRate,                
            };

            await _unitOfWork.GetRepository<Payment>().InsertAsync(firstPayment);
            bool isCreated = await _unitOfWork.CommitAsync() > 0;
            return isCreated;
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
                decimal commissionAmount = appointment.TotalPrice * commissionRate;
                totalCommission += commissionAmount;
            }

            return totalCommission;
        }

        public async Task<ResponsePayment> GetInformationPaymentOfSalon(Guid id)
        {
            var payment = await _unitOfWork.GetRepository<Payment>()
                        .SingleOrDefaultAsync(
                            predicate: p => p.SalonOWnerID == id && p.Status == PaymentStatus.Fake,
                            include: i => i.Include(m => m.SalonOwner).Include(n => n.Config)
                        );

            if (payment == null)
            {
                throw new Exception("Payment not found");
            }

            
            var responsePayment = new ResponsePayment
            {
                Id = payment.Id,
                TotalAmount = (int)await AmountofCommissionRateInMonthBySalon(id, (decimal)0.1),
                PaymentDate = payment.PaymentDate,
                MethodBanking = payment.MethodBanking,
                Description = payment.Description,
                Status = payment.Status,
                PaymentCode = payment.PaymentCode,
                StartDate = payment.StartDate,
                EndDate = payment.EndDate,
                SalonOwners = new SalonOwnerPaymentResponse
                {
                    Id = payment.SalonOwner.Id,
                    FullName = payment.SalonOwner.FullName,
                    Email = payment.SalonOwner.Email,
                    Phone = payment.SalonOwner.Phone,
                    Address = payment.SalonOwner.Address,
                    Img = payment.SalonOwner.Img
                },      
                Config = new ConfigPaymentResponse
                {
                    PakageName = payment.PakageName,
                }
            };

            return responsePayment;


        }
    }
}
