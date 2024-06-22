using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
namespace Hairhub.Domain.Dtos.Responses.Appointments
{

    public class GetAppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public string Status { get; set; }
        public List<GetAppointmentDetailResponse> AppointmentDetails { get; set; } = new List<GetAppointmentDetailResponse>();
        public AppointmentSalon SalonInformation { get; set; }
        public CustomerAppointment Customer { get; set; }
    }

    public class AppointmentSalon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string Img { get; set; }
    }

    public class CustomerAppointment
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Img { get; set; }
    }
}
