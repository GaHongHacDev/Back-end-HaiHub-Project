using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class PaymentReportResponse
    {
        public Guid Id { get; set; }
        public string Beneficiary { get; set; }
        public string NumberAccount { get; set; }
        public string BankName { get; set; }
        public string? ReasonCancle { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? PaymentDate { get; set; }  
        public string? Description { get; set; }
        public string? Typeofpayment { get; set; }
        public string? Statusofpayment { get; set; }
        public decimal? Balance { get; set; }
        public string? Status { get; set; }

        public List<string> urlPaymentImage { get; set; }
        public AccountInformation AccountInformation { get; set; }
    }
    public class AccountInformation
    {
        public Guid? Id { get; set; }

        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }   

        public string Phone {  get; set; }

        public string urlImage { get; set; }

    }


}
