using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Feedbacks
{
    public class CreateFeedbackRequest
    {
        public Guid SalonId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AppointmentId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public List<IFormFile>? ImgFeedbacks { get; set; }
        //public IFormFile? Video { get; set; }
    }
}
