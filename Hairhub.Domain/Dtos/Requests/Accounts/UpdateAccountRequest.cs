using Microsoft.AspNetCore.Http;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class UpdateAccountRequest
    {
        public string? FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public IFormFile? Img { get; set; }
    }
}
