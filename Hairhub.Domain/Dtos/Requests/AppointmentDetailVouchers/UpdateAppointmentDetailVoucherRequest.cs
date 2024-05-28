using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.AppointmentDetailVouchers
{
    public class UpdateAppointmentDetailVoucherRequest
    {
        public Decimal? AppliedAmount { get; set; }
        public DateTime? AppliedDate { get; set; }
    }
}
