using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreatePaymentRequest
    {
        public Guid? ConfigId { get; set; }
        public Guid? AppointmentId { get; set; }

        public Guid? SalonId  { get; set; }

        public decimal Price { get; set; }
        public string? Description { get; set; }

    }

}
