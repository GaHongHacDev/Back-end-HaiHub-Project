using Hairhub.Domain.Dtos.Responses.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Specifications
{
    public class AvailableTimeComparer : IEqualityComparer<AvailableTime>
    {
        public bool Equals(AvailableTime x, AvailableTime y)
        {
            return x.TimeSlot == y.TimeSlot;
        }

        public int GetHashCode(AvailableTime obj)
        {
            return obj.TimeSlot.GetHashCode();
        }
    }
}
