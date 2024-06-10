using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreatePaymentRequest
    {
        public string description { get; set; }
        public string buyerName { get; set; }
        public string buyerEmail { get; set; }

        public string buyerPhone { get; set; }
        public string buyerAddress { get; set; }
        public decimal totalAmount { get; set; }
        public Guid SalonOwnerId { get; set; }

    }
}
