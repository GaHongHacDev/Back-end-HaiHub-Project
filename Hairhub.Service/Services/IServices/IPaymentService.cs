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
        Task<CreatePaymentResult> CreatePaymentUrlRegisterCreator(CreatePaymentRequest request);
        Task<bool> GetPaymentInfo(string paymentLinkId, SavePaymentInfor createPaymentRequest);
        Task<IPaginate<ResponsePayment>> GetPayments(int page, int size);



        Task<bool> CreateFirstTimePayment(Guid salonownerid);


        Task<IPaginate<ResponsePayment>> GetPaymentBySalonOwnerID(Guid ownerid, int page, int size);

    }
}
