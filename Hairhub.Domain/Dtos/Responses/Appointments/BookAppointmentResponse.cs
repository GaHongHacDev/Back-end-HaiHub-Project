using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class BookAppointmentResponse
    {
        public DateTime Day {  get; set; }
        public Guid SalonId {  get; set; }
        public DateTime StartTime { get; set; }
        public List<BookingDetailResponse> BookingDetailResponses { get; set; }
    }

    public class BookingDetailResponse
    {
        public Guid ServiceHairId { get; set; }
        public List<EmployeeAvailable> Employees { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Decimal WaitingTime { get; set; }
    }

    public class EmployeeAvailable
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Img { get; set; }
    }
}
