using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Customers
{
    public class CheckInRequest
    {
        public Guid CustomerId {  get; set; }
        public string DataString { get; set; }
    }
}
