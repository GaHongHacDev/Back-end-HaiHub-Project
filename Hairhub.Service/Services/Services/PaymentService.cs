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

        public PaymentService(IOptions<PayOSSettings> settings, HttpClient client, IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper) 
        {
            _payOSSettings = settings.Value;
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
                int amount = (int)request.Configs.PackageFee;
                var orderCode = new Random().Next(1, 1000000);
                var description = request.Description;
                var clientId = _config["PayOS:ClientId"];
                var apikey = _config["PayOS:APIKey"];
                var checksumkey = _config["PayOS:ChecksumKey"];
                var returnurl = _config["PayOS:ReturnUrl"];
                var returnurlfail = _config["PayOS:ReturnUrlFail"];
                PayOS pos = new PayOS(clientId, apikey, checksumkey);
                // Prepare data for signature
                var signatureData = new Dictionary<string, object>
                 {
                     { "amount", amount },
                     { "cancelUrl", returnurlfail},
                     { "description", description },
                     { "expiredAt", DateTimeOffset.Now.ToUnixTimeSeconds() },
                     { "orderCode", orderCode },
                     { "returnUrl", returnurl}
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
                    cancelUrl: returnurlfail,
                    returnUrl: returnurl,
                    signature: signature,
                    buyerName: request.SalonOwner.FullName,
                    buyerPhone: request.SalonOwner.Phone, // Provide a valid currency
                    buyerEmail: request.SalonOwner.Email,
                    buyerAddress: request.SalonOwner.Address,
                    expiredAt: (int)expiredAt.ToUnixTimeSeconds()
                );
                
                paymentData.items.Add(new ItemData(request.SalonOwner.FullName, 1, amount ));
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
                            var payment = _mapper.Map<Payment>(createPaymentRequest);
                            payment.Id = Guid.NewGuid();
                            payment.TotalAmount = (int)paymentInfo["amount"];
                            payment.PaymentCode = (int)paymentInfo["orderCode"];
                            payment.PaymentDate = DateTime.Now;
                            payment.MethodBanking = "PayOS";
                            payment.Status = status;
                            // Save the transaction
                            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                            isStatus = await _unitOfWork.CommitAsync() > 0;
                            return isStatus;
                            
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

        public async Task<IEnumerable<ResponsePayment>> GetPaymentBySalonOwnerID(Guid salonownerid)
        {
            var existingsalonowner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: e => e.Id == salonownerid);
            if(existingsalonowner == null)
            {
                throw new Exception("Not found");
            }
            var payment = await _unitOfWork.GetRepository<Payment>()
                .GetListAsync(predicate: s => s.SalonOWnerID == salonownerid,
                              include: query => query.Include(s => s.SalonOwner));

            return _mapper.Map<IEnumerable<ResponsePayment>>(payment);
        }
    }
}
