using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AppointmentDetails
{
    public class AvailableEmployeeResponse
    {
        public AvailableEmployeeResponse() 
        {
            Results = new List<AvailableBooking>();
        }
        public List<AvailableBooking> Results { get; set; }
    }

    public class AvailableBooking
    {
        public ServiceHair ServiceHair {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal WaitingTime { get; set; } = 0;
        public bool IsAnyOne { get; set; } = true;
        public List<SalonEmployee> salonEmployees { get; set; }
    }
}
