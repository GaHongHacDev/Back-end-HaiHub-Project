using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class SalonSuggesstionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        public string? Img { get; set; }
        public Decimal? Rate { get; set; }
        public int TotalReviewer { get; set; }
        public string Status { get; set; }

    }
}
