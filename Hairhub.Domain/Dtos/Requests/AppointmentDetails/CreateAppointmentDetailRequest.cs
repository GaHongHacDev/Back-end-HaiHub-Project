using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.AppointmentDetails
{
    public class CreateAppointmentDetailRequest
    {
        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
    }
}
