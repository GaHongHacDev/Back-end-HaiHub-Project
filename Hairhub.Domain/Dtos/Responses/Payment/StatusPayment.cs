using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class StatusPayment
    {
        public string code {  get; set; }

        public string des {  get; set; }

        public string url { get; set; }

        public data Data { get; set; }

    }

    public class data
    {
        public string status { get; set; }

        public decimal amount { get; set; }
    }
}
