using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonEmployees
{
    public class GetSalonEmployeeResponse
    {
        public Guid Id { get; set; }
        public Guid? SalonInformationId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }
        public SalonInformationSalonEmployeeResponse SalonInformation { get; set; }
    }

    public class SalonInformationSalonEmployeeResponse
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public string? BusinessLicense { get; set; }
        public bool? IsActive { get; set; }
    }
}
