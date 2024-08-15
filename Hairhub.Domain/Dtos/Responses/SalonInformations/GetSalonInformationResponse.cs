using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class GetSalonInformationResponse
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public decimal? Rate { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int TotalRating { get; set; }
        public int TotalReviewer { get; set; }
        public int? NumberOfReported { get; set; }
        public string Status { get; set; }

        public virtual SalonOwnerSalonInformationResponse SalonOwner { get; set; }
        public List<GetScheduleResponse> schedules { get; set; }
    }
    public class SalonOwnerSalonInformationResponse
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
