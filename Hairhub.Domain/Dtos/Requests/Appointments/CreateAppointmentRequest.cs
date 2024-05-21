using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class AppointmentDetailRequest
    {
        public AppointmentDetailRequest()
        {
        }

        public AppointmentDetailRequest(Guid? salonEmployeeId, Guid? serviceHairId, string? description, DateTime? date, DateTime? time, decimal? originalPrice, decimal? discountedPrice)
        {
            SalonEmployeeId = salonEmployeeId;
            ServiceHairId = serviceHairId;
            Description = description;
            Date = date;
            Time = time;
            OriginalPrice = originalPrice;
            DiscountedPrice = discountedPrice;
        }

        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
    }
    public class CreateAppointmentRequest
    {
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? IsActive { get; set; }
        public List<AppointmentDetailRequest>?  ListAppointmentDetail { get; set; }
    }
}
