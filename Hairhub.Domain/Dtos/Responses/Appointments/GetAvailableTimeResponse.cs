using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAvailableTimeResponse
    {
        public List<AvailableTime> AvailableTimes { get; set; } = new List<AvailableTime>();
    }

    public class AvailableTime
    {
        public decimal TimeSlot { get; set; }
        public List<EmployeeAvailable> employeeAvailables { get; set; } = new List<EmployeeAvailable>();
    }
}
