using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid ConfigId { get; set; }
        public Guid SalonOWnerID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string MethodBanking { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int PaymentCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual SalonOwner SalonOwner { get; set; }
        public virtual Config Config { get; set; }
    }
}
