using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class SalonStatus
    {
        public static string Approved = "APPROVED";
        public static string Rejected = "REJECTED";
        public static string Pending = "PENDING";
        public static string Edited = "EDITED";
        public static string Unavailable = "UNAVAILABLE";
        public static string Creating = "CREATING";
    }
}
