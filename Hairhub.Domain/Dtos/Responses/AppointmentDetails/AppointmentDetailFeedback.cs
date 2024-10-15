using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AppointmentDetails
{
    public class AppointmentDetailFeedback
    {
            public Guid Id { get; set; }
            public DateTime EndTime { get; set; }
            public DateTime StartTime { get; set; }
            public int Feedback {  get; set; }
            //public int Rating {  get; set; } 

            //Service Hair
            public string? ServiceName { get; set; }
            public string? DescriptionServiceHair { get; set; }
            public decimal? PriceServiceHair { get; set; }
            public decimal? TimeServiceHair { get; set; }
    }
}
