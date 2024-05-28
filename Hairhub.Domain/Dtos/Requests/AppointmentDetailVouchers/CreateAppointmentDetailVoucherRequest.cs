using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.AppointmentDetailVouchers
{
    public class CreateAppointmentDetailVoucherRequest
    {
        public Guid Id { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Decimal? AppliedAmount { get; set; }
        public DateTime? AppliedDate { get; set; }
    }
}
