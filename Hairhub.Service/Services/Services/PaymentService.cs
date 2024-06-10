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

namespace Hairhub.Service.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;
        private readonly HttpClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IAppointmentService _apppointmentservice;

        public PaymentService(IOptions<PayOSSettings> settings, HttpClient client, IUnitOfWork unitOfWork, IConfiguration config, IAppointmentService apppointmentservice)
        {
            _payOSSettings = settings.Value;
            //_payOS = new PayOS(_payOSSettings.ClientId, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);
            _client = client;
            _unitOfWork = unitOfWork;
            _config = config;
            _apppointmentservice = apppointmentservice;
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
                
                int amount = (int)request.totalAmount;
                var orderCode = new Random().Next(1, 1000000);
                var description = request.description;
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
                    buyerName: request.buyerName,
                    buyerPhone: request.buyerPhone, // Provide a valid currency
                    buyerEmail: request.buyerEmail,
                    buyerAddress: request.buyerAddress,
                    expiredAt: (int)expiredAt.ToUnixTimeSeconds()
                );
                
                //await SavePaymentInfo(new Payment
                //{
                //    Id = Guid.NewGuid(), // Generate new ID for the payment record
                //    CustomerId = request.items.CustomerId,
                //    TotalAmount = amount,
                //    PaymentDate = DateTime.UtcNow,
                //    MethodBanking = "PayOS",
                //    SalonId = request.items.SalonId,
                //    Description = description,
                //    Status = "Pending",
                //    PaymentCode = orderCode,
                //});
                paymentData.items.Add(new ItemData(request.buyerName, 1, amount ));
                var createPaymentResult = await pos.createPaymentLink(paymentData);

                return createPaymentResult; // Chú ý sử dụng PaymentLink thay vì paymentLink

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        

        public async Task<string> GetPaymentInfo(string paymentLinkId, CreateAppointmentRequest appointmentrequest)
        {
            var getUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{paymentLinkId}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, getUrl);
                request.Headers.Add("x-client-id", _config["PayOS:ClientId"]);
                request.Headers.Add("x-api-key", _config["PayOS:APIKey"]);

                var response = await _client.SendAsync(request);

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
                            // Extract necessary details from the payment info
                            var orderCode = paymentInfo["orderCode"];
                            var amount = paymentInfo["amount"];
                            var payment = new Payment {
                                Id = Guid.NewGuid(),
                                PaymentCode = (int)orderCode,
                                //CustomerId = appointmentrequest.CustomerId,
                                TotalAmount = (int)amount,
                                MethodBanking = "PayOS",
                                Description = paymentInfo["transactions"][0]["description"]?.ToString(),
                                Status = paymentInfo["status"]?.ToString(),
                                PaymentDate = DateTime.Parse(paymentInfo["transactions"][0]["transactionDateTime"].ToString()),
                                //SalonId = Guid.NewGuid(),
                                };

                            // Save the transaction
                            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                            


                           // await _apppointmentservice.CreateAppointment(appointmentrequest);
                            _unitOfWork.Commit();

                            return "Payment has been successfully received and saved.";
                        }
                        else if (status == "pending")
                        {
                            return "Payment is still pending.";
                        }
                        else
                        {
                            return $"Payment status is {status}.";
                        }
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

        //public async Task<IPaginate<ResponsePayment>> GetPaymant(int page, int size)
        //{
        //    IPaginate<ResponsePayment> payment = await _unitOfWork.GetRepository<Payment>()
        //        .GetPagingListAsync(selector: x => new ResponsePayment(x.Id, x.CustomerId, x.TotalAmount, x.PaymentDate
        //        , x.MethodBanking, x.SalonId, x.Description), page: page, size: size, orderBy: x => x.OrderBy(x => x.PaymentDate));

        //    return payment;
        //}
    }
}
