using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.AppointmentDetails
{
    public class AvailableEmployeeRequest
    {
        public Guid SalonId { get; set; }
        public DateTime BookDate { get; set; }
        public List<Guid> ServiceHairIds {  get; set; }
    }
}
