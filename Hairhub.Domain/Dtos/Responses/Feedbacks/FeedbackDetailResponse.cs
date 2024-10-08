using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Feedbacks
{
    public class FeedbackDetailResponse
    {
        public Guid AppointmentDetailId { get; set; }  

        public Guid FeedbackId { get; set; }

        public int Rating { get; set; }
    }
}
