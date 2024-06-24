using System.ComponentModel.DataAnnotations;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CreateAccountRequest
    {
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
    }
}
