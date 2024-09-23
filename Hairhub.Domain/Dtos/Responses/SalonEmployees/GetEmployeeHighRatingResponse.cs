using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonEmployees
{
    public class GetEmployeeHighRatingResponse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string FullName { get; set; } //
        public string? Gender { get; set; } //
        public string? Phone { get; set; }
        public string Img { get; set; } //
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int? RatingCount { get; set; }
        public int? RatingSum { get; set; }
        public decimal? Rating { get; set; }
        public bool IsActive { get; set; }
    }
}
