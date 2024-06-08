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
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? EndOperationalHours { get; set; }
        public DateTime? StartOperationalHours { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }
        public List<CreateSalonScheduleRequest> Schedules { get; set; }
    }
    public class CreateSalonScheduleRequest
    {
        public Guid? SalonId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
