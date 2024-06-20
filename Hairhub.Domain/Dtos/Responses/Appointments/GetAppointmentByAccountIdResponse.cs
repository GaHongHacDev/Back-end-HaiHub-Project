using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAppointmentByAccountIdResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public string Status { get; set; }
        public List<GetAppointmentDetailResponse> AppointmentDetails { get; set; } = new List<GetAppointmentDetailResponse>();
    }
}
