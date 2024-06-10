using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class StaticFile
    {
        public Guid Id { get; set; }
        public Guid? FeedbackId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }

        public virtual Feedback Feedback { get; set; }
    }
}
