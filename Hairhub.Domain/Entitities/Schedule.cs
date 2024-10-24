﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? SalonId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive {  get; set; }

        public virtual SalonEmployee SalonEmployee { get; set; }
        public virtual SalonInformation SalonInformation { get; set; }
    }
}
