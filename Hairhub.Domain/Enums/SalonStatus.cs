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
        public static string Suspended = "SUSPENDED"; // bị report 4 lần => Nộp tiền để comeback
        public static string Creating = "CREATING";
        public static string OverDue = "OVERDUE"; // Khi salon bị quá hạn đóng tiền 
        public static string Disable = "DISABLE"; // bị report 5 lần => Xóa khỏi hệ thống || Không đóng tiền gia hạn || Tự nguyện dừng salon
    }
}
