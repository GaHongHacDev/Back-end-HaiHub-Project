using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class GetCalculatePriceRequest
    {
        public Guid? VoucherId { get; set; }   
        public List<Guid?> ServiceHairId { get; set; }
    }
}
