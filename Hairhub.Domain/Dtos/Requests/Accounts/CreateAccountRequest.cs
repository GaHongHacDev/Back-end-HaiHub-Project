using System.ComponentModel.DataAnnotations;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CreateAccountRequest
    {
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DayOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
