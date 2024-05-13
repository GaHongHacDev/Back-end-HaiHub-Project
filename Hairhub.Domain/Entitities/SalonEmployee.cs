﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonEmployee
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public string? Address { get; set; }
        public string? humanId { get; set; }
        public string? Img { get; set; }

        public SalonInformation SalonInformation { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }

    }
}
