using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Otps
{
    public class SendOtpEmailResponse
    {
        public string? Email { get; set; }
        public string? OtpKey { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TypeOtp { get; set; }
    }
}
