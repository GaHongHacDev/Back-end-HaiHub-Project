using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class AppointmentFeedback
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Status { get; set; }
        public bool IsFeedback { get; set; }
        public List<AppointmentDetailFeedback> AppointmentDetails { get; set; } = new List<AppointmentDetailFeedback>();
    }
}
