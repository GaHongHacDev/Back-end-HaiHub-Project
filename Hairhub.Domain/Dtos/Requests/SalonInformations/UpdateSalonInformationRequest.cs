using Hairhub.Domain.Entitities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class UpdateSalonInformationRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image {  get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
    }
}
