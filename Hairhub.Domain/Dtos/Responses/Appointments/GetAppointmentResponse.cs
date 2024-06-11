using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{

    public class GetAppointmentResponse
    {
        public GetAppointmentResponse() { }


        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public string Status { get; set; }
        public CustomerResponse Customer { get; set; }
        public List<GetAppointmentDetailResponse> AppoinmentDetails { get; set; } = new List<GetAppointmentDetailResponse> { };
    }

    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
    }

}
