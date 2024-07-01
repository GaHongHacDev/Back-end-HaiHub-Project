using Hairhub.Domain.Dtos.Requests.StaticFile;
using Hairhub.Domain.Dtos.Responses.StaticFile;
using Hairhub.Domain.Entitities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Reports
{
    public class CreateReportRequest
    {
        public Guid SalonId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AppointmentId { get; set; }
        public string RoleNameReport { get; set; } // Customer or SalonOwner
        public string ReasonReport {  get; set; }
        public List<IFormFile> ImgeReportRequest { get; set; }
    }
}
