using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class AppointmentStatus
    {
        public static string Booking = "BOOKING";
        public static string CancelByCustomer = "CANCEL_BY_CUSTOMER";
        public static string Fail = "FAILED";
        public static string Successed = "SUCCESSED";
    }
}
