using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class NotificationDetail
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid AccountId { get; set; }
        public Guid AppointmentId { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadDate { get; set; }

        public virtual Notification Notification { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual Account Account { get; set; }

    }
}
