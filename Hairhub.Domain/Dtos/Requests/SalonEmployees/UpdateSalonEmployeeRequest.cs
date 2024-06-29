using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonEmployees
{
    public class UpdateSalonEmployeeRequest
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public string Img { get; set; }
        public bool IsActive { get; set; }
    }
}
