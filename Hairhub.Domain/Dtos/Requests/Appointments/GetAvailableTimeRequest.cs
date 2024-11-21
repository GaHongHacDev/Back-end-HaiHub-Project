using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class GetAvailableTimeRequest
    {
        public DateTime Day {  get; set; }
        public Guid SalonId { get; set; }
        public Guid ServiceHairId { get; set; }
        public Guid? SalonEmployeeId {  get; set; }
        public bool IsAnyOne { get; set; }
    }
}
