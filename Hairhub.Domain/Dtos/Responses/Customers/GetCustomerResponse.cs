using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Customers
{
    public class GetCustomerResponse
    {
        public GetCustomerResponse()
        {
            
        }

        public GetCustomerResponse(Guid? id, Guid? accountId, string? fullName, DateTime? dayOfBirth, string? gender, string? email, string? phone, string? address, string? img, string? bankAccount, string? bankName)
        {
            Id = id;
            AccountId = accountId;
            FullName = fullName;
            DayOfBirth = dayOfBirth;
            Gender = gender;
            Email = email;
            Phone = phone;
            Address = address;
            Img = img;
            BankAccount = bankAccount;
            BankName = bankName;
        }

        public Guid? Id { get; set; }
        public Guid? AccountId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
    }
}
