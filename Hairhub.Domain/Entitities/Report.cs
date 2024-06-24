using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid? SalonId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid AppointmentId { get; set; }
        public string RoleNameReport { get; set; } // Customer or SalonOwner
        public DateTime CreateDate { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public string? DescriptionAdmin { get; set; }
        public string Status { get; set; }

        public virtual SalonInformation SalonInformation { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<StaticFile> StaticFiles { get; set; }
    }
}
