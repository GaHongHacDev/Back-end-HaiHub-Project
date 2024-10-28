using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class QueryRequest
    {
        public string? Paymentlink { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? des { get; set; }

        public string? accountid { get; set; }

        public string? appontmentid { get; set; }
        public string? configid { get; set; }
        public decimal price { get; set; }

        public int orderCode { get; set; }
        public string? Url { get; set; }
    }
}
