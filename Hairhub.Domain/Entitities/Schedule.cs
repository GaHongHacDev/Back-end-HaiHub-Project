using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsActive {  get; set; }

        public virtual SalonEmployee SalonEmployee { get; set; }
    }
}
