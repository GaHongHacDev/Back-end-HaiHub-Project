﻿using Hairhub.Domain.Dtos.Requests.Payment;
using Hairhub.Domain.Dtos.Responses.Payment;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Net.payOS.Types;
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
        Task<string> GetPaymentInfo(string paymentLinkId);

        Task<IPaginate<ResponsePayment>> GetPaymant(int page, int size);
    }
}
