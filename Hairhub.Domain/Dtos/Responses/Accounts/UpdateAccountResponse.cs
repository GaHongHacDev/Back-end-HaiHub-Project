using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Accounts
{
    public class UpdateAccountResponse
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RoleName { get; set; }
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
