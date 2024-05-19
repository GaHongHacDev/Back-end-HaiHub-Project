using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? IsActive { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }
    }
}
