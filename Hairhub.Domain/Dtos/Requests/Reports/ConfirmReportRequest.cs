using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Reports
{
    public class ConfirmReportRequest
    {
        public string? DescriptionAdmin { get; set; }
        public string Status {  get; set; }
    }
}
