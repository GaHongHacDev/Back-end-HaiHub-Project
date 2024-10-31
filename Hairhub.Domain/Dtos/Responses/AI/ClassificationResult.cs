using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AI
{
    public class ClassificationResult
    {
        public string? Intent { get; set; }
        public string? TyleGuid { get; set; }
        public string? StatusAppointment { get; set; }
        public string? Position { get; set; }
        public string? SalonName { get; set; }
        public DateTime? Time { get; set; }
    }
}
