using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonInformation
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public Decimal? Rate { get; set; }
        public string Longitude {  get; set; }
        public string Latitude {  get; set; }
        public bool IsActive { get; set; }

        public virtual SalonOwner SalonOwner { get; set; }
        public virtual ICollection<SalonEmployee> SalonEmployees { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }  
        public virtual ICollection<ServiceHair> ServiceHairs { get; set; }
    }
}
