using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Reports
{
    public class UpdateReportRequest
    {
        public Guid? SalonId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid AppointmentId { get; set; }
        public string RoleNameReport { get; set; } // Customer or SalonOwner
        public string? ReasonReport { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public string? DescriptionAdmin { get; set; }
        public string Status { get; set; }
    }
}
