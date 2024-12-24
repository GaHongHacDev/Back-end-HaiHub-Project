using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class GetCustomerQuantityResponse
    {
        public CustomerToday CustomerToday { get; set; }

    }

    public class CustomerToday
    {
        public int NumberOfCustomerToday { get; set; }
        public int NumberOfNewCustomer { get; set; }
        public int NumberOfOldCustomer { get; set; }
        public double NewCustomerPercent { get; set; }
        public double OldCustomerPercent { get; set; }
    }


}
