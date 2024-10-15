using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Notification
{
    public class NotificationResponse
    {
        public NotificationContent Notification {  get; set; }
        public AppointmentContent Appointment { get; set; }
    }


    public class NotificationContent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; }
    }

    public class AppointmentContent
    {
        public Guid Id { get; set; }
        public DateTime ServiceTime { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedDate { get; set; }
    }


}
