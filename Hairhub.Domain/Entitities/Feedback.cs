using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AppointmentId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<StaticFile> StaticFiles { get; set; }
    }
}
