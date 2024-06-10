using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class AppointmentStatus
    {
        public static string Booking = "Booking";
        public static string CancelBySalon = "CANCEL_BY_SALON";
        public static string CancelByCustomer = "CANCEL_BY_CUSTOMER";
        public static string Fail = "FAIL";
        public static string Successed = "SUCCESSED";
    }
}
