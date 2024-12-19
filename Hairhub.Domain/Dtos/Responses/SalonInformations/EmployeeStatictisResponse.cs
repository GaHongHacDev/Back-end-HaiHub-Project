using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class EmployeeStatictisResponse
    {
        public List<EmployeeStatictis> EmployeeStatictis { get; set; } = new List<EmployeeStatictis>();
    }
    public class EmployeeStatictis
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public long NumberOfService { get; set; }
        public long NumberOfUsers { get; set; }
        public decimal Revenue { get; set; }
    }
}
