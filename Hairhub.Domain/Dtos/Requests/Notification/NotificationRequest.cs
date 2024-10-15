using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Notification
{
    public class NotificationRequest
    {
        public Guid? AppointmentId { get; set; }
        public string? Title { get; set; }

        public string? Message { get; set; }

        public string? Type { get; set; }
    }
}
