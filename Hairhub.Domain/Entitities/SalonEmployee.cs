﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonEmployee
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender {get; set; }
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }

        public SalonInformation SalonInformation { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }

    }
}