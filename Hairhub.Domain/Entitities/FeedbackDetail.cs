using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class FeedbackDetail
    {
        [Key]
        public Guid AppointmentDetailId { get; set; }  // Khóa chính

        public Guid FeedbackId { get; set; }  // Khóa ngoại đến Feedback

        public int Rating { get; set; }

        public virtual Feedback Feedback { get; set; }  // Quan hệ đến Feedback
        public virtual AppointmentDetail AppointmentDetail { get; set; }  // Quan hệ đến AppointmentDetail
    }
}
