using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Otps
{
    public class SendOTPRequestWithdraw
    {
        public Guid AccountId { get; set; }
    }
}
