using Hairhub.Domain.Dtos.Requests.VnPay;
using Hairhub.Domain.Dtos.Responses.VnPay;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IVNPayService 
    {
        //string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        //PaymentResponseModel PaymentExecute(IQueryCollection collections);
        Task<string> CreatePaymentUrl(VnPayRequest request, string returnUrl, string clientIPAddress);

        Task<bool> ConfirmPayment(string queryString,  string orderInfor,  string responseCode);
    }
}
