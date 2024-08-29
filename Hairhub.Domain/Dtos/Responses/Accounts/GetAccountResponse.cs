using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Accounts
{
    public class GetAccountResponse
    {
        public string AccountId { get; set; }
        public Guid? RoleId { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
    }
}
