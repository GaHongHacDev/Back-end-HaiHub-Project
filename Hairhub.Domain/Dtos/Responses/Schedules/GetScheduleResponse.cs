using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Schedules
{
    public class GetScheduleResponse
    {
        public Guid Id { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool? IsActive { get; set; }
    }

    //public class SalonEmployeeResponseS
    //{
    //    public Guid Id { get; set; }
    //    public Guid SalonInformationId { get; set; }
    //    public string? FullName { get; set; }
    //    public DateTime? DayOfBirth { get; set; }
    //    public string? Gender { get; set; }
    //    public string? Email { get; set; }
    //    public string? Phone { get; set; }
    //    public string? Img { get; set; }
    //    public bool? IsActive { get; set; }
    //}

    //public class SalonInformationSchedule
    //{
    //    public Guid Id { get; set; }
    //    public Guid AccountId { get; set; }
    //    public string FullName { get; set; }
    //    public DateTime DayOfBirth { get; set; }
    //    public string Gender { get; set; }
    //    public string Email { get; set; }
    //    public string? Phone { get; set; }
    //    public string? Address { get; set; }
    //    public string? Img { get; set; }
    //}
}
