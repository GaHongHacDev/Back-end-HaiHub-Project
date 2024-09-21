using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Feedbacks
{
    public class FeedbackDetailRequest
    {
        public Guid AppointmentDetailId { get; set; }
        public int Rating { get; set; }
    }
}
