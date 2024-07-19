using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Reports;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using Hairhub.Domain.Dtos.Responses.StaticFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Feedbacks
{
    public class GetFeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? AppointmentDetailId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public GetAppointmentResponse Appointment { get; set; }
        public List<FileFeedbackResponse> FileFeedbacks { get; set; }
        public CustomerAppointment Customer { get; set; }
    }

    public class FileFeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid? FeedbackId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }

}
