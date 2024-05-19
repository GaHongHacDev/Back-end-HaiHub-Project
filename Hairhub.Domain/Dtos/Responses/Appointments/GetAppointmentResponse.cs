using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class CustomerInfomation()
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
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

    public class GetAppointmentResponse
    {
        public GetAppointmentResponse(Guid id, CustomerInfomation customerInfomation, DateTime? date, decimal? totalPrice, bool? isActive)
        {
            Id = id;
            CustomerInfomation = customerInfomation;
            Date = date;
            TotalPrice = totalPrice;
            IsActive = isActive;
        }

        public Guid Id { get; set; }
        public CustomerInfomation CustomerInfomation { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
    }
}
