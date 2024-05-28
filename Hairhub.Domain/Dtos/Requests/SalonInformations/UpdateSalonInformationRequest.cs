using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class UpdateSalonInformationRequest
    {
        public Guid? OwnerId { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? EndOperationalHours { get; set; }
        public DateTime? StartOperationalHours { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }

        public virtual SalonOwner SalonOwner { get; set; }
    }
}
