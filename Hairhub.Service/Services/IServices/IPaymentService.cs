using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Dtos.Responses.Payment;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Net.payOS.Types;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IPaymentService
    {
        Task<bool> FakePaymentForCommissionRate(SavePaymentInfor createPaymentRequest);
        Task<bool> PromotionPaymentForCommissionRate(SavePaymentInfor createPaymentRequest);
        Task<decimal> AmountofCommissionRateInMonthBySalon(Guid id, decimal commisionrate);
        Task<CreatePaymentResult> SendPaymentLink(Guid accountId, CreatePaymentRequest request);
        Task<bool> ConfirmPayment(string queryString, string paymentlinkId, Guid accountid, decimal price, Guid? configid);
        Task<IPaginate<PaymentHistory>> GetPaymentHistory(DateTime? payDate, Guid? accountId, string? email,
                                                             string? paymentType, string? status, int page = 1, int size = 10);


        Task<PaymentReportResponse> GetPaymentReportById(Guid id);


    }
}
