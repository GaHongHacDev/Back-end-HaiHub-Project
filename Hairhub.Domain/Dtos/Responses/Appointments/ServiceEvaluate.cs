using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class ServiceEvaluate
    {
        public List<EvaluateService>? evaluatedServices {  get; set; }
    }

    public class EvaluateService
    {
        public string? ServiceName { get; set; }

        public string? Number { get; set; }
    }
}
