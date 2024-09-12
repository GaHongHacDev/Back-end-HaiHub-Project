using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public static class AppointmentStatus
    {
        public const string Booking = "BOOKING";
        public const string CancelByCustomer = "CANCEL_BY_CUSTOMER";
        public const string Fail = "FAILED";
        public const string Successed = "SUCCESSED";
    }
}
