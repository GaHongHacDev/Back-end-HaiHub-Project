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
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid CustomerId { get; set; }
        public bool? IsActive { get; set; }
        public CustomerResponse Customer { get; set; }
    }

    public class CustomerResponse
    {
        public CustomerResponse() { }

        public CustomerResponse(Guid id, Guid? accountId, DateTime? dayOfBirth, string? gender, string? fullName, string? email, string? phone, string? address, string? humanId, string? img, string? bankAccount, string? bankName)
        {
            Id = id;
            AccountId = accountId;
            DayOfBirth = dayOfBirth;
            Gender = gender;
            FullName = fullName;
            Email = email;
            Phone = phone;
            Address = address;
            HumanId = humanId;
            Img = img;
            BankAccount = bankAccount;
            BankName = bankName;
        }

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
