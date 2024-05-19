using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AppointmentDetails
{
    public class AppointmentDetailResponse
    {
        public AppointmentDetailResponse(Guid id, Guid? salonEmployeeId, Guid? serviceHairId, Guid? appointmentId, string? description, DateTime? date, DateTime? time, decimal? originalPrice, decimal? discountedPrice, bool? status)
        {
            Id = id;
            SalonEmployeeId = salonEmployeeId;
            ServiceHairId = serviceHairId;
            AppointmentId = appointmentId;
            Description = description;
            Date = date;
            Time = time;
            OriginalPrice = originalPrice;
            DiscountedPrice = discountedPrice;
            Status = status;
        }

        public Guid Id { get; set; }
        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? Status { get; set; }
    }
}
