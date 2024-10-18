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
        string CreatePaymentUrl(double amount, string infor, string orderinfor, string returnUrl, string clientIPAddress);

        bool ConfirmPayment(string queryString, out string orderInfor, out string responseCode);
    }
}
