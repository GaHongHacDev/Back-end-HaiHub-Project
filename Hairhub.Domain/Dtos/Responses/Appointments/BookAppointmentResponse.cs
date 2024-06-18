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
        public List<BookingDetailResponse> BookingDetailResponses { get; set; } = new List<BookingDetailResponse>();
    }

    public class BookingDetailResponse
    {
        public ServiceHairAvalibale ServiceHair { get; set; }
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

    public class ServiceHairAvalibale
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Img { get; set; }
        public decimal Time { get; set; }
        public bool IsActive { get; set; }
    }
}
