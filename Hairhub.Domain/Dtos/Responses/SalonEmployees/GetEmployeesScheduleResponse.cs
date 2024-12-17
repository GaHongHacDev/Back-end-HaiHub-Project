using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonEmployees
{
    public class GetEmployeesScheduleResponse
    {
        public List<EmployeesSchedule> employeesSchedules { get; set; } = new List<EmployeesSchedule>();
    }

    public class EmployeesSchedule
    {
        public Guid Id { get; set; }
        public string FullName {  get; set; }
        public string? Gender { get; set; } 
        public string? Phone { get; set; }
        public string Img { get; set; } 
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public List<WorkSchedule> WorkSchedules { get; set; }
    }

    public class WorkSchedule
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Note { get; set; }
        public string Type {  get; set; }
    }
}
