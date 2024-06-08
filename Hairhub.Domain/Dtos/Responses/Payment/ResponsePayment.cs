using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class ResponsePayment
    {
        public ResponsePayment() { }

        public ResponsePayment(Guid? id, Guid? customerId, decimal? totalAmount,
            DateTime? paymentDate, string? methodBanking, Guid? salonId,
            string? description)
        {
            Id = id;
            CustomerId = customerId;
            TotalAmount = totalAmount;
            PaymentDate = paymentDate;
            MethodBanking = methodBanking;
            SalonId = salonId;
            Description = description;
        }

        public Guid? Id { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? MethodBanking { get; set; }
        public Guid? SalonId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int PaymentCode { get; set; }

        // Navigation properties
        public virtual CustomerInformation Customer { get; set; }
        public virtual SalonInfor SalonInformation { get; set; }
    }

    public class CustomerInformation
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class SalonInfor
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}
