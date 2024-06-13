using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonEmployees
{
    public class GetSalonEmployeeResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string Img { get; set; }
        public bool IsActive { get; set; }
        public List<ScheduleEmployeeResponse> Schedules { get; set; }
        public List<GetServiceHairResponse>  ServiceHairs { get; set; }
    }

    public class ScheduleEmployeeResponse
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
