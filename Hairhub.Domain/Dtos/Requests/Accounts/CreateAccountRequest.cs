using System.ComponentModel.DataAnnotations;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CreateAccountRequest
    {
        [Required]
        public string RoleName { get; set; }
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        public string Phone {  get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự")]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
    }
}
