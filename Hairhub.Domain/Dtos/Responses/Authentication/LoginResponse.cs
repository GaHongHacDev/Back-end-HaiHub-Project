using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Authentication
{
    public class LoginResponse
    {
        public string AccessToken {  get; set; } 
        public string RefreshToken {  get; set; }
        public Guid AccountId { get; set; }
        public CustomerLoginResponse? CustomerResponse {  get; set; }
        public SalonOwnerLoginResponse? SalonOwnerResponse {  get; set; }
    }

    public class CustomerLoginResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public DateTime DayOfBirth { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
    }
    public class SalonOwnerLoginResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public DateTime DayOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
    }
}
