using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreateFirstTimePaymentRequest
    {
        public Guid SalonOwnerId {  get; set; }
    }
}
