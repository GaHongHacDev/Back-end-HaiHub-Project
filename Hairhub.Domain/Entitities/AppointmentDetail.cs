using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class AppointmentDetail
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalonEmployeeId { get; set; }
        public Guid ServiceHairId { get; set; }
        public Guid AppointmentId { get; set; }
        public string? Description {  get; set; } 
        public DateTime? Date {  get; set; }
        public DateTime? Time { get; set; }
        public Decimal? Price { get; set; }
        public bool? Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SalonEmployee SalonEmployee { get; set; }
        public virtual ServiceHair ServiceHair { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
