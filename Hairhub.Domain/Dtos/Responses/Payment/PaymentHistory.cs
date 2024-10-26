using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class PaymentHistory
    {
        public Guid Id { get; set; }
        public Guid? ConfigId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string email { get; set; }
        public string RoleName { get; set; }
    }
}
