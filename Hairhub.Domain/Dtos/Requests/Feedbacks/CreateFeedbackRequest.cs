using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Feedbacks
{
    public class CreateFeedbackRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? AppointmentDetailId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }
}
