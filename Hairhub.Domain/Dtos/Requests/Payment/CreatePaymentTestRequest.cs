using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreatePaymentTestRequest
    {
        public Guid? ConfigId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
