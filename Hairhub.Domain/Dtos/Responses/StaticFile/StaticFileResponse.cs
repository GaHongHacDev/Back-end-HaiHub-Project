using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.StaticFile
{
    public class StaticFileResponse
    {
        public Guid Id { get; set; }
        public Guid? FeedbackId { get; set; }
        public Guid? ReportId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }
}
