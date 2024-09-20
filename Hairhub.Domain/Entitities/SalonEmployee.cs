using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonEmployee
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string FullName { get; set; } //
        public string? Gender {get; set; } //
        public string? Phone {  get; set; }
        public string Img { get; set; } //
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } //
        public Guid? AccountId { get; set; }
        public int? RatingCount { get; set; }
        public int? RatingSum { get; set; }
        public decimal? Rating {  get; set; }

        public Account? Account { get; set; }
        public SalonInformation SalonInformation { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }
        public virtual ICollection<ServiceEmployee> ServiceEmployees { get; set; }

    }
}
