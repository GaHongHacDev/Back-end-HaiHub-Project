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
        public decimal? Rate { get; set; }
        public decimal Longitude {  get; set; }
        public decimal Latitude {  get; set; }
        public int TotalRating {  get; set; }
        public int TotalReviewer {  get; set; }
        public int? NumberOfReported { get; set; }
        public string Status { get; set; }

        public virtual SalonOwner SalonOwner { get; set; }
        public virtual ICollection<SalonEmployee> SalonEmployees { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }  
        public virtual ICollection<ServiceHair> ServiceHairs { get; set; }
        public virtual ICollection<Approval> Approvals { get; set; }
        public virtual ICollection<Report> Report { get; set; }
    }
}
