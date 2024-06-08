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
        public Guid? CustomerId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? MethodBanking { get; set; }

        public Guid? SalonId { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; }

        public int PaymentCode { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }

        public virtual SalonInformation SalonInformation { get; set; } 
    }
}
