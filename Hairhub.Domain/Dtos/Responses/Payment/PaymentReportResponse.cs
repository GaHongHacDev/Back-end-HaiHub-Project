using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class PaymentReportResponse
    {
        public string? ReasonCancle { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string FullName { get; set; }
        public string NumberAccount { get; set; }
        public string BankName { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }

        public PaymentInformation Payment { get; set; }
    }

    public class PaymentInformation{
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string? PaymentCode { get; set; } 

        public SalonOwnerInformation salon {  get; set; }

        public CustomerInformation Customer { get; set; }
    }

    public class SalonOwnerInformation
    {
        public string Phone { get; set; }
        public string FullName { get; set; }

        public string? Email { get; set; }
    }

    public class CustomerInformation
    {
        public string Phone { get; set; }
        public string FullName { get; set; }

        public string? Email { get; set; }
    }

}
