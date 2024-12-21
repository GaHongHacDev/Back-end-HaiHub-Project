using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class CompileAppointmentSalonResponse
    {
        public string AppointmentType { get; set; }
        public long AppointmentQuantity { get; set; }
        public long CustomerQuantity { get; set; }
        public decimal Revenue { get; set; }
        public double Percent {  get; set; }
    }
}
