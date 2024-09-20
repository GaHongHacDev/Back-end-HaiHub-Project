using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class ReviewRevenueReponse
    {
        public decimal totalRevenue {  get; set; }
        public IList<ReviewRevenueEmployee> employees { get; set; } = new List<ReviewRevenueEmployee>();
    }
}
