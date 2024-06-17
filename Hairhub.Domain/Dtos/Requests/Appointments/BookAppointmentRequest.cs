using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class BookAppointmentRequest
    {
        public DateTime Day { get; set; }
        public Decimal AvailableSlot { get; set; }
        public Guid SalonId { get; set; }
        public List<BookingDetailRequest> BookingDetail { get; set; }
    }

    public class BookingDetailRequest 
    {
        public Guid ServiceHairId { get; set; }
        public Guid? SalonEmployeeId { get; set; }
        public bool IsAnyOne {  get; set; }
    }
}
