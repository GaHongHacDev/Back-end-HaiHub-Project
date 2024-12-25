using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class GetEmployeeEvaluationResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public decimal Rate { get; set; }
    }
}
