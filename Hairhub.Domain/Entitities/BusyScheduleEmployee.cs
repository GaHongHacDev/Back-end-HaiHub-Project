using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class BusyScheduleEmployee
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Note {  get; set; }
        public string Status { get; set; }

        public virtual SalonEmployee SalonEmployee { get; set; }
    }
}
