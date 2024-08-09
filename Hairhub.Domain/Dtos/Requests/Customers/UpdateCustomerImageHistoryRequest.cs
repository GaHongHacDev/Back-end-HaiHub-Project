using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Customers
{
    public class UpdateCustomerImageHistoryRequest
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public virtual List<IFormFile>? ImageStyles { get; set; }

        public List<Guid>? RemoveImageStyleIds { get; set; }

        //public bool DeleteExistingStyles { get; set; } = false;
    }
}
