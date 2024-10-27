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
        Task<StatusPayment> ConfirmPayment(string queryString, QueryRequest requestquery);
        Task<IPaginate<PaymentHistory>> GetPaymentHistory(DateTime? payDate, Guid? accountId, string? email,
                                                             string? paymentType, string? status, int page = 1, int size = 10);
        Task<bool> CreateWithdrawPayment(CreateWithdrawPaymentRequest request);
        Task<bool> ConfirmWithdrawPayment(Guid id, WithdrawConfirmRequest request);

        Task<IPaginate<GetPaymentReportReponse>> GetPaymentReport(Guid? accountId, string? email, DateTime? createDate, string? status, int page, int size);
        Task<PaymentReportResponse> GetPaymentReportById(Guid id);


    }
}
