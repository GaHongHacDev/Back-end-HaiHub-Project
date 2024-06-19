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
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? humanId { get; set; }
        public string? Img { get; set; }
        public bool? isActive { get; set; }
    }
    public class ServiceHairResponse
    {
        public Guid Id { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public Guid SalonInformationId { get; set; }
        public bool? isActive { get; set; }
    }

    public class GetAppointmentDetailResponse
    {
        public Guid Id { get; set; }
        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? Status { get; set; }

        public SalonEmployeeResponse SalonEmployeeResponse { get; set; }
        public ServiceHairResponse ServiceHairResponse { get; set; }
    }
}
