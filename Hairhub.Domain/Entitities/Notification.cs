using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }

        public virtual ICollection<NotificationDetail> NotificationDetails { get; set; } = new List<NotificationDetail>();
    }
}
