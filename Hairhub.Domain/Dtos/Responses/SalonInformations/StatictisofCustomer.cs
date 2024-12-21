using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class StatictisofCustomer
    {
        public Guid? CustomerID { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }
        
        public int? NumberofSuccessAppointment { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
