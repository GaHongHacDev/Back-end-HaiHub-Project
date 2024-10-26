using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class AppointmentDetail
    {
        public Guid Id { get; set; }
        public Guid SalonEmployeeId { get; set; }
        public Guid ServiceHairId { get; set; }
        public Guid AppointmentId { get; set; }
        public string? Description {  get; set; } 
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        
        //Service Hair
        public string? ServiceName { get; set; }
        public string? DescriptionServiceHair { get; set; }
        public decimal? PriceServiceHair { get; set; }
        public string? ImgServiceHair { get; set; }
        public decimal? TimeServiceHair { get; set; }


        public virtual SalonEmployee SalonEmployee { get; set; }
        public virtual ServiceHair ServiceHair { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual FeedbackDetail FeedbackDetail { get; set; }
    }
}
