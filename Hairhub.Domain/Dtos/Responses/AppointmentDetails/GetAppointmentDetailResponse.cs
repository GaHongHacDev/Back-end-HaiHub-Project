using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AppointmentDetails
{

    public class SalonEmployeeResponse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string FullName { get; set; } //
        public string? Gender { get; set; } //
        public string? Phone { get; set; }
        public string Img { get; set; } //
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } //
        public Guid? AccountId { get; set; }
    }

    public class ServiceHairResponse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Img { get; set; }
        public decimal Time { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetAppointmentDetailResponse
    {
        public Guid Id { get; set; }
        public Guid SalonEmployeeId { get; set; }
        public Guid ServiceHairId { get; set; }
        public Guid AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }

        //Service Hair
        public string? ServiceName { get; set; }
        public string? DescriptionServiceHair { get; set; }
        public decimal? PriceServiceHair { get; set; }
        public string? ImgServiceHair { get; set; }
        public decimal? TimeServiceHair { get; set; }

        public SalonEmployeeResponse SalonEmployee { get; set; }
    }
}
