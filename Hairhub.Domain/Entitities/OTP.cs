using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class OTP
    {
        [Key]
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OtpKey { get; set; }
        public DateTime? CreatedTime { get; set; }
        public double? ExpireTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? TypeOtp {  get; set; }
    }
}
