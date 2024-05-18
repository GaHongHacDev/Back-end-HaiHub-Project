using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAppointmentResponse
    {
        public GetAppointmentResponse(Guid id, DateTime? date, decimal? totalPrice, Guid customerId, bool? isActive)
        {
            Id = id;
            Date = date;
            TotalPrice = totalPrice;
            CustomerId = customerId;
            IsActive = isActive;
        }

        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid CustomerId { get; set; }
        public bool? IsActive { get; set; }
    }
}
