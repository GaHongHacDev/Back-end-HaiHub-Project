using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.ServiceHairs
{
    public class GetServiceHairResponse
    {
        public Guid Id { get; set; }
        public Guid? SalonInformationId { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Time { get; set; }
        public bool? IsActive { get; set; }
    }
}
