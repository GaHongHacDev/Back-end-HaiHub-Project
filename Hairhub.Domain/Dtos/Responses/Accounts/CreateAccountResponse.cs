namespace Hairhub.Domain.Dtos.Responses.Accounts
{
    public class CreateAccountResponse
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RoleName { get; set; }
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
