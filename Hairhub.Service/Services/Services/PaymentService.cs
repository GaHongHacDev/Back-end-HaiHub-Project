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
using System.Net;
using Hairhub.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Http;
using Hairhub.Common.ThirdParties.Contract;

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
        private readonly IMediaService _mediaService;
        private readonly IEmailService _emailService;

        public PaymentService(IOptions<PayOSSettings> settings, HttpClient client, IUnitOfWork unitOfWork, IConfiguration config, 
                                IMapper mapper, IAppointmentService appointmentService, IMediaService mediaService, IEmailService emailService)
        {
            _payOSSettings = settings.Value;
            _appointmentservice = appointmentService;
            _mediaService = mediaService;
            _emailService = emailService;
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
                var Configs = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: c => c.Id == request.ConfigId);               
                var SalonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: s => s.Id == request.SalonOWnerID);
                if (SalonOwner == null)
                {
                    throw new Exception("SalonOwner is null.");
                }
                

                var Salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: s => s.SalonOwner.Id == request.SalonOWnerID);
                if (Salon == null)
                {
                    throw new Exception("Salon is null.");
                }
                int amount = (int)await AmountofCommissionRateInMonthBySalon(SalonOwner.Id, (decimal)Configs.CommissionRate);
                string currentTimeString = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                long orderCode = long.Parse(currentTimeString.Substring(currentTimeString.Length - 6));
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
                    expiredAt: (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds()
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
            throw new NotImplementedException();
            /* var getUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{paymentLinkId}";

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
                                 var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.SalonOWnerID == createPaymentRequest.SalonOwnerId && p.Status == PaymentStatus.Fake);
                                 payment.Status = PaymentStatus.Paid;
                                 // Save the transaction
                                 _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
                                 await PaymentForCommissionRate(createPaymentRequest);
                                 await _unitOfWork.CommitAsync();
                                 return isStatus = true;

                             } else if( salon.Status == SalonStatus.OverDue) {
                                 var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
                                 var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.SalonOWnerID == createPaymentRequest.SalonOwnerId && p.Status == PaymentStatus.Fake);
                                 payment.Status = PaymentStatus.Paid;
                                 salon.Status = SalonStatus.Approved;

                                 // Save the transaction
                                 _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);
                                 _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
                                 await PaymentForCommissionRate(createPaymentRequest);
                                 await _unitOfWork.CommitAsync();
                                 return isStatus = true;
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
            */
        }

        public async Task<IPaginate<ResponsePayment>> GetPaymentBySalonOwnerID(Guid ownerid, int page, int size)
        {
            throw new NotImplementedException();
            /*
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

            return paginateResponse;*/
        }

        public async Task<IPaginate<ResponsePayment>> GetPayments(string? valueSearch, int page, int size)
        {
            throw new NotImplementedException();
            /*
            var payments = await _unitOfWork.GetRepository<Payment>()
                .GetPagingListAsync(predicate: x => x.Status == PaymentStatus.Paid 
                                                    && (string.IsNullOrEmpty(valueSearch) 
                                                    || x.SalonOwner.Email!.ToLower().Contains(valueSearch.Trim().ToLower()) 
                                                    || x.SalonOwner.SalonInformations.Any(x=>x.Name.ToLower().Contains(valueSearch.Trim().ToLower()))),
                include: query => query.Include(x => x.SalonOwner)
                                        .ThenInclude(x => x.SalonInformations)
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

            return paginateResponse;*/
        }

        public async Task<bool> CreateFirstTimePayment(CreateFirstTimePaymentRequest createFirstTimePaymentRequest)
        {
            throw new NotImplementedException();
            /*
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
            return isCreated;*/
        }

        public async Task<bool> PaymentForCommissionRate(SavePaymentInfor createPaymentRequest)
        {
            throw new NotImplementedException();
            /*var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
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
            return isCreated;*/
        }



        public async Task<decimal> AmountofCommissionRateInMonthBySalon(Guid id, decimal commissionRate)
        {
            throw new NotImplementedException();
            /* var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: p => p.SalonOwner.Id == id);
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

             return totalCommission;*/
        }

        public async Task<ResponsePayment> GetInformationPaymentOfSalon(Guid id)
        {
            throw new NotImplementedException();
            /*var payment = await _unitOfWork.GetRepository<Payment>()
                        .SingleOrDefaultAsync(
                            predicate: p => p.SalonOWnerID == id && p.Status == PaymentStatus.Fake,
                            include: i => i.Include(m => m.SalonOwner).Include(n => n.Config)
                        );
            if (payment == null)
            {
                throw new NotFoundException("Payment not found");
            }
            if (payment.EndDate.Date > DateTime.Now.Date)
            {
                throw new NotFoundException($"Chưa tới ngày thanh toán, ngày thanh toán của bạn là {payment.EndDate}");
            }
            var responsePayment = new ResponsePayment
            {
                Id = payment.Id,
                TotalAmount = (int)await AmountofCommissionRateInMonthBySalon(id, (decimal)payment.CommissionRate!),
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
                    Id = payment.Id,
                }
            };
            payment.TotalAmount = responsePayment.TotalAmount;
            payment.Id = responsePayment.Id;
            _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await _unitOfWork.CommitAsync();
            return responsePayment;*/


        }

        public async Task<IPaginate<PaymentHistory>> GetPaymentHistory(DateTime? payDate, Guid? accountId, string? email, string? paymentType, string? status, int page = 1, int size = 10)
        {
            email = (email == null || email.Trim() == "") ? "" : email;
            paymentType = (paymentType == null || paymentType.Trim() == "") ? "" : paymentType;
            status = (status == null || status.Trim() == "") ? "" : status;

            var predicate = PredicateBuilder.New<Payment>(x => x.Status.Equals(status) && x.PaymentType.Equals(paymentType) && x.Account.UserName.Contains(email));
            if(accountId != null)
            {
                predicate = predicate.And(x => x.AccountId == accountId);
            }
            if (payDate.HasValue)
            {
                predicate = predicate.And(x => x.PaymentDate!.Value.Date == payDate.Value.Date);
            }

            var payments = await _unitOfWork.GetRepository<Payment>()
                                            .GetPagingListAsync(
                                                predicate: predicate,
                                                include: x=>x.Include(s=>s.Account).Include(s=>s.Account.Role).Include(s=>s.Account.SalonOwners)
                                                             .Include(s => s.Account.Customers),
                                                page: page, 
                                                size: size
                                            );
            var paginateResponse = new Paginate<PaymentHistory>
            {
                Page = payments.Page,
                Size = payments.Size,
                Total = payments.Total,
                TotalPages = payments.TotalPages,
                Items = _mapper.Map<IList<PaymentHistory>>(payments.Items)
            };

            return paginateResponse; 
        }

        public async Task<bool> CreateWithdrawPayment(CreateWithdrawPaymentRequest request)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x=>x.Id == request.AccountId && x.IsActive);
            if (account == null)
            {
                throw new NotFoundException($"Không tìm thấy account với id {request.AccountId}");
            }
            if (account.Balance<request.Balance)
            {
                throw new Exception("Số dư trong ví không đủ");
            }
            else if (request.Balance < 10000)
            {
                throw new Exception("Không đủ số tiền để rút từ ví");
            }

            account.Balance-= request.Balance;
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);

            Payment payment = new Payment()
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                TotalAmount = request.Balance,
                PaymentType = PaymentType.Withdraw,
                Description = request.Description,
                Status = PaymentStatus.Pending
            };
            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);

            PaymentReport paymentReport = new PaymentReport()
            {
                PaymentId = payment.Id,
                CreateDate = DateTime.UtcNow,
                FullName = request.FullName,
                NumberAccount = request.NumberAccount,
                BankName = request.BankName,
                Balance = request.Balance,
                Status = PaymentStatus.Pending
            };
            await _unitOfWork.GetRepository<PaymentReport>().InsertAsync(paymentReport);


            List<StaticFile> staticFile = new List<StaticFile>();

            for (int i=1; i<=request.IdentityCard.Count; i++) 
            {
                var url = await _mediaService.UploadAnImage(request.IdentityCard.ElementAt(i-1), MediaPath.PAYMENT_REPORT_IMG, 
                                                            paymentReport.PaymentId.ToString() + i.ToString());
                staticFile.Add(new StaticFile()
                {
                    Id = Guid.NewGuid(),
                    PaymentReportId = paymentReport.PaymentId,
                    Img = url,
                });
            }
            await _unitOfWork.GetRepository<StaticFile>().InsertRangeAsync(staticFile);
            bool isCommit = await _unitOfWork.CommitAsync()>0;
            return isCommit;
        }

        public async Task<bool> ConfirmWithdrawPayment(WithdrawConfirmRequest request)
        {
            if (request.StatusConfirm.Equals(PaymentStatus.Cancel))
            {
                if (string.IsNullOrEmpty(request.ReasonCancel!.Trim()))
                {
                    throw new NotFoundException("Không tìm thấy lý do từ chối");
                }
                var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: x => x.Id == request.Id);
                if (payment == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment với id {request.Id}");
                }
                payment.Status = PaymentStatus.Cancel;
                _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);

                var paymentReport = await _unitOfWork.GetRepository<PaymentReport>().SingleOrDefaultAsync(predicate: x=>x.PaymentId == request.Id);
                if (paymentReport == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment report với id {request.Id}");
                }
                paymentReport.ReasonCancle = request.ReasonCancel;
                paymentReport.ConfirmDate = DateTime.UtcNow;
                paymentReport.Status = PaymentStatus.Cancel;
                _unitOfWork.GetRepository<PaymentReport>().UpdateAsync(paymentReport);

                bool isUpdate = await _unitOfWork.CommitAsync()>0;
                return isUpdate;
            }
            else if(request.StatusConfirm.Equals(PaymentStatus.Paid))
            {
                var payment = await _unitOfWork.GetRepository<Payment>()
                                                .SingleOrDefaultAsync(
                                                    predicate: x => x.Id == request.Id, 
                                                    include: x=>x.Include(s=>s.Account).ThenInclude(s=>s.Role)
                                                );
                if (payment == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment với id {request.Id}");
                }
                payment.Status = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;

                var paymentReport = await _unitOfWork.GetRepository<PaymentReport>().SingleOrDefaultAsync(predicate: x => x.PaymentId == request.Id);
                if (paymentReport == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment report với id {request.Id}");
                }
                paymentReport.ConfirmDate = DateTime.UtcNow;
                paymentReport.Status = PaymentStatus.Paid;
                _unitOfWork.GetRepository<PaymentReport>().UpdateAsync(paymentReport);
                
                string urlImg = await _mediaService.UploadAnImage(request.BankingImgs, MediaPath.PAYMENT_BANKED_IMG, paymentReport.PaymentId.ToString());

                string fullName = "";
                if(payment.Account.Role.RoleName!.Equals(RoleEnum.Customer.ToString()))
                {
                    var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x=>x.AccountId == payment.AccountId);
                    fullName = customer.FullName;
                }
                else if (payment.Account.Role.RoleName!.Equals(RoleEnum.SalonOwner.ToString()))
                {
                    var salon = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == payment.AccountId);
                    fullName = salon.FullName;
                }
                bool isSendMail = await _emailService.SendConfirmWithdraw(payment.Account.UserName, "Hairhub thông báo rút tiền thành công", fullName, 
                                                        DateTime.Now.Date.ToString(), paymentReport.FullName, paymentReport.NumberAccount, paymentReport.BankName,
                                                        paymentReport.Balance.ToString(), urlImg);
                if (!isSendMail)
                {
                    throw new NotFoundException("Gửi mail thất bại. Vui lòng thử lại sau!");
                }
                bool isCommit = await _unitOfWork.CommitAsync()>0;
                return isCommit;
            }
            else
            {
                throw new NotFoundException("Trạng thái duyệt đơn không đúng");
            }
        }
    }
}
