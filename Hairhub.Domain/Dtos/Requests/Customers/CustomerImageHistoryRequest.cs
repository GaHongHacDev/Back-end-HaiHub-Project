using Hairhub.Domain.Entitities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Customers
{
    public class CustomerImageHistoryRequest
    {
        public Guid? CustomerId { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public virtual List<IFormFile>? ImageStyles { get; set; }
    }
}
