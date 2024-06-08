using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class CreateSalonInformationRequest
    {
        public Guid? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public string? BusinessLicense { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool? IsActive { get; set; }
        public List<CreateSalonScheduleRequest> SalonInformationSchedules { get; set; }
    }
    public class CreateSalonScheduleRequest
    {
        public string Date {  get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
