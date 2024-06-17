using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class CreateAppointmentRequest
    {
        public Guid CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public List<AppointmentDetailRequest>  AppointmentDetails { get; set; }
        public List<Guid>? VoucherIds { get; set; }
    }

    public class AppointmentDetailRequest
    {
        public Guid SalonEmployeeId { get; set; }
        public Guid ServiceHairId { get; set; }
        public Guid AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
    }

}
