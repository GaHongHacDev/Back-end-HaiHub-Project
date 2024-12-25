using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class GetCustomerQuantityResponse
    {
        public CustomerDate CustomerDate { get; set; } = new CustomerDate();

        public List<ChartCustomer> CharCustomer { get; set; } = new List<ChartCustomer>();
    }

    public class CustomerDate
    {
        public int TotalCustomer { get; set; }
        public int NumberOfNewCustomer { get; set; }
        public int NumberOfOldCustomer { get; set; }
        public double NewCustomerPercent { get; set; }
        public double OldCustomerPercent { get; set; }
    }

    public class ChartCustomer
    {
        public int Time { get; set; }
        public int NumberOfCustomer { get; set; }
    }
}
