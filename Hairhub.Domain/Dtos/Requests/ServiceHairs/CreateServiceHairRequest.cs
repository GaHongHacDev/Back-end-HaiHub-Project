using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.ServiceHairs
{
    public class CreateServiceHairRequest
    {
        public Guid SalonInformationId { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile Img { get; set; }
        public decimal? Time { get; set; }
        public bool? IsActive { get; set; }
    }
}
