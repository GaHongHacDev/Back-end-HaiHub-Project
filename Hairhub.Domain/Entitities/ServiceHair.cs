using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class ServiceHair
    {
        public Guid Id { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Img { get; set; }
        public decimal? Time { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }
        public virtual ICollection<ServiceEmployee> ServiceEmployees { get; set; }
    }
}
