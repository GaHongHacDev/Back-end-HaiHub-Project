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
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Http;
using Hairhub.Common.ThirdParties.Contract;
using System.Drawing;
using CloudinaryDotNet;
using System.Security.Principal;


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
        public async Task<bool> FakePaymentForCommissionRate(SavePaymentInfor createPaymentRequest)
        {
           
            var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
            Guid account = createPaymentRequest.AccountId;
            var firstPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Description = "Tiền hoa hồng dựa trên lịch hẹn tháng đầu tiên",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PaymentDate = DateTime.Now,
                AccountId = account,
                PaymentCode = "",
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

        public async Task<bool> PromotionPaymentForCommissionRate(SavePaymentInfor createPaymentRequest)
        {

            var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == createPaymentRequest.ConfigId);
            Guid account = createPaymentRequest.AccountId;
            var firstPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Description = "Tiền hoa hồng dựa trên lịch hẹn tháng đầu tiên",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PaymentDate = DateTime.Now,
                AccountId = account,
                PaymentCode = "",
                Status = PaymentStatus.Promotion,
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

             var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.AccountId == salon.SalonOwner.AccountId && p.Status == PaymentStatus.Fake);
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
        public async Task<CreatePaymentResult> SendPaymentLink(Guid accountId, CreatePaymentRequest request)
        {
            try
            {
                string returnUrl = $"https://hairhub.gahonghac.net/api/v1/payment/PaymentConfirm?accountId={accountId}&amount={request.Price}&config={request.ConfigId}&appointment={request.AppointmentId}";

                
                var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: p => p.Id == accountId);
                if (account == null) throw new Exception("account not null!!");


                int amount = (int)request.Price;
                string currentTimeString = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                long orderCode = long.Parse(currentTimeString.Substring(currentTimeString.Length - 6));
                var description = request.Description;
                string? clientId = _config["PayOS:ClientId"];
                var apikey = _config["PayOS:APIKey"];
                var checksumkey = _config["PayOS:ChecksumKey"];
                var returnurlfail = _config["PayOS:ReturnUrlFail"];

                PayOS pos = new PayOS(clientId, apikey, checksumkey);
                
                var signatureData = new Dictionary<string, object>
                 {
                     { "amount", amount },
                     { "cancelUrl", returnurlfail},
                     { "description", description },
                     { "expiredAt", DateTimeOffset.Now.ToUnixTimeSeconds() },
                     { "orderCode", orderCode },
                     { "returnUrl", returnUrl}
                 };
                var sortedSignatureData = new SortedDictionary<string, object>(signatureData);
                var dataForSignature = string.Join("&", sortedSignatureData.Select(p => $"{p.Key}={p.Value}"));
                var signature = ComputeHmacSha256(dataForSignature, checksumkey);
                DateTimeOffset expiredAt = DateTimeOffset.Now.AddMinutes(10);

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    items: new List<ItemData>(), // Provide a list of items if needed
                    cancelUrl: returnurlfail,
                    returnUrl: returnUrl,
                    signature: signature,
                    buyerName: account.UserName,
                    expiredAt: (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds()
                );

                paymentData.items.Add(new ItemData(account.UserName, 1, amount));
                var createPaymentResult = await pos.createPaymentLink(paymentData);
                string url = createPaymentResult.checkoutUrl;
                return createPaymentResult; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }



        public async Task<StatusPayment> ConfirmPayment(string queryString, QueryRequest requestquery)
        {
            
            var getUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{requestquery.Paymentlink}";
            try
            {
                Guid? appointmentId = Guid.TryParse(requestquery.appontmentid, out var appointmentGuid) ? appointmentGuid : (Guid?)null;
                Guid? accountid = Guid.TryParse(requestquery.accountid, out var accountGuid) ? accountGuid : (Guid?)null;
                Guid? configid = Guid.TryParse(requestquery.configid, out var configGuid) ? configGuid : (Guid?)null;
                var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == configid);
                var appointment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: p => p.Id == appointmentId);
                var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: p => p.Id == accountid);
                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, getUrl);
                request.Headers.Add("x-client-id", _config["PayOS:ClientId"]);
                request.Headers.Add("x-api-key", _config["PayOS:APIKey"]);              
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
                            
                            var balance = account.Balance;
                            account.Balance = balance + requestquery.price;
                            _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
                            
                            if (config != null)
                            {
                                var fakepayment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: p => p.Status == PaymentStatus.Fake);
                                fakepayment.Status = PaymentStatus.Paid;
                                fakepayment.TotalAmount = requestquery.price;
                                fakepayment.PaymentDate = DateTime.UtcNow;
                                fakepayment.Description = $"Thanh toán thành công tiền hoa hồng tháng {fakepayment.PaymentDate.Value.Month - 1}";
                                _unitOfWork.GetRepository<Payment>().UpdateAsync(fakepayment);
                                var nextpayment = new SavePaymentInfor
                                {
                                    AccountId = (Guid)accountid,
                                    ConfigId = config.Id,
                                };
                                await FakePaymentForCommissionRate(nextpayment);
                            }
                            else if (appointment != null) {
                                var paymentAppointment = new Payment
                                {
                                    Id = Guid.NewGuid(),    
                                    AccountId = (Guid)accountid,
                                    AppointmentId = appointment.Id,
                                    ConfigId = config == null ? null : config.Id,
                                    Description = $"Nạp tiền thành công vào ví",
                                    PaymentDate = DateTime.Now,
                                    TotalAmount = requestquery.price,
                                    Status = PaymentStatus.Paid,
                                    PaymentCode = requestquery.Paymentlink,
                                    PaymentType = PaymentType.Deposit,   
                                    
                                };
                                await _appointmentservice.UpdateAppointmentFakeById(appointment.Id);
                                await _unitOfWork.GetRepository<Payment>().InsertAsync(paymentAppointment);
                            } else if (appointment == null && config == null)
                            {
                                var paymentWallet = new Payment
                                {
                                    Id = Guid.NewGuid(),
                                    AccountId = (Guid)accountid,
                                    AppointmentId = null,
                                    ConfigId = null,
                                    Description = $"Nạp tiền thành công vào ví{DateTime.Now}",
                                    PaymentDate = DateTime.Now,
                                    TotalAmount = requestquery.price,
                                    Status = PaymentStatus.Paid,
                                    PaymentCode = requestquery.Paymentlink,
                                    PaymentType = PaymentType.Deposit,

                                };
                                await _unitOfWork.GetRepository<Payment>().InsertAsync(paymentWallet);
                            }
                            var tran = new StatusPayment
                            {
                                code = requestquery.Code,
                                des = "Thành công rùi nè",                                
                                Data = new data
                                {
                                    status = requestquery.Status,
                                    amount = requestquery.price
                                                                    }
                            };
                            isStatus = await _unitOfWork.CommitAsync() > 0;
                            return tran;
                        }
                        await _appointmentservice.DeleteAppointmentFakeById(appointment.Id);
                        return null!;
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
        
        public async Task<IPaginate<PaymentHistory>> GetPaymentHistory(DateTime? payDate, Guid? accountId, string? email, string? paymentType, string? status, int page = 1, int size = 10)
        {
            email = (email == null || email.Trim() == "") ? "" : email;
            paymentType = (paymentType == null || paymentType.Trim() == "") ? "" : paymentType;
            status = (status == null || status.Trim() == "") ? "" : status;

            var predicate = PredicateBuilder.New<Payment>(x => x.Status.Contains(status) && x.PaymentType.Contains(paymentType) && x.Account.UserName.Contains(email) 
                                                            && !x.Status.Equals(PaymentStatus.Fake));
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
                                                include: x=>x.Include(s=>s.Account).Include(s=>s.Account.Role),
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
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x=>x.Id == request.AccountId && x.IsActive);
            if (account == null)
            {
                throw new NotFoundException($"Không tìm thấy account với id {request.AccountId}");
            }
            if (account.Balance<request.Balance)
            {
                throw new Exception("Số dư trong ví không đủ");
            }
            else if (request.Balance < 50000)
            {
                throw new Exception("Không đủ số tiền để rút từ ví");
            }

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

        public async Task<bool> ConfirmWithdrawPayment(Guid Id, WithdrawConfirmRequest request)
        {
            if (request.StatusConfirm.Equals(PaymentStatus.Cancel))
            {
                if (string.IsNullOrEmpty(request.ReasonCancel!.Trim()))
                {
                    throw new NotFoundException("Không tìm thấy lý do từ chối");
                }
                var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(predicate: x => x.Id == Id);
                if (payment == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment với id {Id}");
                }
                payment.Status = PaymentStatus.Cancel;
                _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);

                var paymentReport = await _unitOfWork.GetRepository<PaymentReport>().SingleOrDefaultAsync(predicate: x=>x.PaymentId == Id);
                if (paymentReport == null) 
                {
                    throw new NotFoundException($"Không tìm thấy payment report với id {Id}");
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
                                                    predicate: x => x.Id == Id
                                                );
                if (payment == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment với id {Id}");
                }
                payment.Status = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;
                _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);

                var paymentReport = await _unitOfWork.GetRepository<PaymentReport>().SingleOrDefaultAsync(predicate: x => x.PaymentId == Id);
                if (paymentReport == null)
                {
                    throw new NotFoundException($"Không tìm thấy payment report với id {Id}");
                }

                var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>()
                                                .SingleOrDefaultAsync(
                                                    predicate: x => x.Id == payment.AccountId, 
                                                    include: x=>x.Include(s=>s.Role)
                                                );
                if (account == null)
                {
                    throw new NotFoundException($"Không tìm thấy tài khoản với id {payment.AccountId}");
                }
                if (paymentReport.Balance <= account.Balance)
                {
                    account.Balance -= paymentReport.Balance;
                    _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
                }
                else
                {
                    throw new NotFoundException("Số dư tài khoản không đủ để rút tiền");
                }

                paymentReport.ConfirmDate = DateTime.UtcNow;
                paymentReport.Status = PaymentStatus.Paid;
                _unitOfWork.GetRepository<PaymentReport>().UpdateAsync(paymentReport);
                
                string urlImg = await _mediaService.UploadAnImage(request.BankingImgs, MediaPath.PAYMENT_BANKED_IMG, paymentReport.PaymentId.ToString());

                string fullName = "";
                if(account.Role.RoleName!.Equals(RoleEnum.Customer.ToString()))
                {
                    var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x=>x.AccountId == account.Id);
                    fullName = customer.FullName;
                }
                else if (account.Role.RoleName!.Equals(RoleEnum.SalonOwner.ToString()))
                {
                    var salon = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                    fullName = salon.FullName;
                }
                bool isSendMail = await _emailService.SendConfirmWithdraw(account.UserName, "Hairhub thông báo rút tiền thành công", fullName, 
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


        public async Task<PaymentReportResponse> GetPaymentReportById(Guid id)
        {
            var paymentReport = await _unitOfWork.GetRepository<PaymentReport>().
                                      SingleOrDefaultAsync(predicate: p => p.PaymentId == id,
                                                           include: i => i.Include(p => p.Payment)
                                                           .ThenInclude(p => p.Account)
                                                           .ThenInclude(p => p.Customers));
            if (paymentReport == null) { throw new NotFoundException("Không tìm thấy thông tin của payment"); }
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>()
                                .SingleOrDefaultAsync(predicate: p => p.Id == paymentReport.Payment.AccountId,
                                include: i => i.Include(p => p.Customers).Include(p => p.SalonOwners)
                                );
            if (account == null) { throw new NotFoundException("Không tìm thấy thông tin của account"); }
            var salonOwner = await _unitOfWork.GetRepository<SalonOwner>()
                                .SingleOrDefaultAsync(predicate: p => p.AccountId == paymentReport.Payment.AccountId,
                                include: i => i.Include(p => p.SalonInformations)
                                );
            var CustomerInformation = await _unitOfWork.GetRepository<Customer>()
                                .SingleOrDefaultAsync(predicate: p => p.AccountId == paymentReport.Payment.AccountId
                                );
            var staticfile = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: p => p.PaymentReportId == paymentReport.PaymentId);

            List<string> Images = new List<string>();
            if (staticfile != null)
            {
                foreach (var image in staticfile)
                {
                    Images.Add(image.Img);
                }
            }

            if (staticfile == null) { throw new NotFoundException("Không tìm thấy thông tin của staticfile  "); }
            string email = "", phone="", Name="", url ="", roleName ="";
            Guid? userid = null;
            if (salonOwner != null)
            {
                userid = salonOwner.Id;
                email = salonOwner.Email!;
                phone = salonOwner.Phone;
                Name = salonOwner.FullName;
                url = salonOwner.Img!;
                roleName = RoleEnum.SalonOwner.ToString();
            } else if (CustomerInformation != null){
                userid = CustomerInformation.Id;
                email = CustomerInformation.Email!;
                phone = CustomerInformation.Phone;
                Name = CustomerInformation.FullName;
                url = CustomerInformation.Img!;
                roleName = RoleEnum.Customer.ToString();
            }
            else
            {
                if (paymentReport != null) { throw new NotFoundException("Không tìm thấy thông tin của salon và customer"); }
            }

            var paymentWithdraw = new PaymentReportResponse
            {
                AccountInformation = new AccountInformation
                {
                    Id = account.Id,
                    UserId = userid ?? Guid.Empty,
                    Email = email,
                    Phone = phone,
                    FullName = Name,
                    urlImage = url,
                    RoleName = roleName,
                    Balance = account.Balance,
                },
                Id = paymentReport!.PaymentId,
                Beneficiary = paymentReport.FullName,
                Balance = paymentReport.Balance,
                ConfirmDate = paymentReport.ConfirmDate,
                CreateDate = paymentReport.CreateDate,
                NumberAccount = paymentReport.NumberAccount,
                BankName = paymentReport.BankName,
                ReasonCancle = paymentReport.ReasonCancle,
                Status = paymentReport.Status,
                Description = paymentReport.Payment.Description,
                PaymentDate = paymentReport.Payment.PaymentDate,
                Statusofpayment = paymentReport.Payment.Status,
                Typeofpayment = paymentReport.Payment.PaymentType,
                urlPaymentImage = Images,
            };

            return paymentWithdraw;
        }


        public async Task<IPaginate<GetPaymentReportReponse>> GetPaymentReport(Guid? accountId, string? email, DateTime? createDate, string? status, int page, int size)
        {
            email = (email == null || email.Trim() == "") ? "" : email;
            status = (status == null || status.Trim() == "") ? "" : status;

            var predicate = PredicateBuilder.New<PaymentReport>(x => x.Status.Contains(status) && x.Payment.Account.UserName.Contains(email));
            if (accountId != null)
            {
                predicate = predicate.And(x => x.Payment.AccountId == accountId);
            }
            if (createDate.HasValue)
            {
                predicate = predicate.And(x => x.CreateDate.Date == createDate.Value.Date);
            }

            var payments = await _unitOfWork.GetRepository<PaymentReport>()
                                            .GetPagingListAsync(
                                                predicate: predicate,
                                                include: x => x.Include(s => s.Payment.Account).Include(s => s.Payment.Account.Role),
                                                page: page,
                                                size: size
                                            );
            var paginateResponse = new Paginate<GetPaymentReportReponse>
            {
                Page = payments.Page,
                Size = payments.Size,
                Total = payments.Total,
                TotalPages = payments.TotalPages,
                Items = _mapper.Map<IList<GetPaymentReportReponse>>(payments.Items)
            };

            return paginateResponse;
        }

    }
}
