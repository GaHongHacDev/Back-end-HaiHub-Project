using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class PaymentStatus
    {
        public static string Cancel = "CANCEL";
        public static string Paid = "PAID";
        public static string Salon_Paid = "SALON_PAID";
        public static string Pending = "PENDING";
        public static string Fake = "FAKE";
        public static string Promotion = "PROMOTION";
    }
}
