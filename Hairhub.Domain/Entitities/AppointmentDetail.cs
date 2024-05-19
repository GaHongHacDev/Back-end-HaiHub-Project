﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class AppointmentDetail
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Description {  get; set; } 
        public DateTime? Date {  get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? Status { get; set; }


        public virtual SalonEmployee SalonEmployee { get; set; }
        public virtual ServiceHair ServiceHair { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<AppointmentDetailVoucher> AppointmentDetailVouchers { get; set; }
    }
}
