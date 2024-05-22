using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class AppointmentDetailVoucher
    {
        public Guid Id { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Decimal? AppliedAmount { get; set; }
        public DateTime? AppliedDate { get; set; }

        public virtual Voucher Voucher { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
