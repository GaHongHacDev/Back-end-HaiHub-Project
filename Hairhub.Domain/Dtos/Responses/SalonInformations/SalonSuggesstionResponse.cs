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
        public decimal? Rate { get; set; }
        public int TotalReviewer { get; set; }
        public string Status { get; set; }

        public List<FileSalonInformationSuggestionResponse> FileSalonInformation {  get; set; }

    }

    public class FileSalonInformationSuggestionResponse
    {
        public Guid Id { get; set; }
        public Guid? SalonInformationId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }
}
