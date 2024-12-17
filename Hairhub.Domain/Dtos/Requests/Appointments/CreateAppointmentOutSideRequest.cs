using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class CreateAppointmentOutSideRequest
    {
        public Guid? CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Phone {  get; set; }
        public string? Email { get; set; }

        public DateTime StartDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public List<AppointmentDetailRequest> AppointmentDetails { get; set; }
    }
}
