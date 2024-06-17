using Hairhub.Domain.Entitities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonEmployees
{
    public class CreateSalonEmployeeRequest
    {
        public Guid SalonInformationId { get; set; }
        public List<EmployeeRequest> SalonEmployees { get; set; }
    }

    public class EmployeeRequest
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public IFormFile ImgEmployee { get; set; }
        public bool IsActive { get; set; };
        public List<Guid> ServiceHairId { get; set; }
        public List<ScheduleEmployee> ScheduleEmployees { get; set; }
    }

    public class ScheduleEmployee
    {
        public string? Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
