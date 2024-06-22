using Hairhub.Domain.Entitities;
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
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string MethodBanking { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int PaymentCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public SalonOwnerPaymentResponse SalonOwners { get; set; }
        public ConfigPaymentResponse Config { get; set; }
        public SalonPaymentResponse SalonInformation { get; set; }
    }

    public class SalonOwnerPaymentResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
    }

    public class ConfigPaymentResponse
    {
        public Guid Id { get; set; }
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal PakageFee { get; set; }
    }

    public class SalonPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string Name { get; set; }
        public string? Img { get; set; }
        public string Status { get; set; }

    }
}
