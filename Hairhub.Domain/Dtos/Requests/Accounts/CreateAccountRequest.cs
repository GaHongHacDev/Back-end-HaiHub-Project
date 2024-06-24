using System.ComponentModel.DataAnnotations;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CreateAccountRequest
    {
        public string RoleName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
