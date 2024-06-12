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
        public ResponsePayment() { }

        public ResponsePayment(Guid? id, Guid? configId, Guid? salonOWnerID,
            decimal? totalAmount, DateTime? paymentDate, string? methodBanking,
            string? description, string? status, int? paymentcode)
        {
            Id = id;
            ConfigId = configId;
            SalonOWnerID = salonOWnerID;
            TotalAmount = totalAmount;
            PaymentDate = paymentDate;
            MethodBanking = methodBanking;
            Description = description;
            Status = status;
            PaymentCode = paymentcode;
        }

        public Guid? Id { get; set; }
        public Guid? ConfigId { get; set; }
        public Guid? SalonOWnerID { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? MethodBanking { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? PaymentCode { get; set; }

        // Navigation properties
        public  SalonOwner SalonOwners { get; set; }
        public  Config Config { get; set; }
    }

    public class SalonOwner
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime DayOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class Config
    {
        public Guid Id { get; set; }
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal PakageFee { get; set; }
    }
}
