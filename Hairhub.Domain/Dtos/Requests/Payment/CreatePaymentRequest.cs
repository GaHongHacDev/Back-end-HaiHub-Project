using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreatePaymentRequest
    {
        public string description { get; set; }
        public string buyerName { get; set; }
        public string buyerEmail { get; set; }

        public string buyerPhone { get; set; }
        public string buyerAddress { get; set; }
        public Appointment items { get; set; }

    }

    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid? CustomerId { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? IsActive { get; set; }

        public Guid? SalonId { get; set; }
    }



}
