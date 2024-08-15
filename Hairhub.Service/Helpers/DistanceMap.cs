using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Helpers
{
    public static class DistanceMap
    {
        private const decimal EarthRadius = 6371m; // Radius of the Earth in kilometers

        public static decimal GetDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            try
            {
                // Convert decimal degrees to radians
                decimal dLat = DegreesToRadians(lat2 - lat1);
                decimal dLon = DegreesToRadians(lon2 - lon1);

                // Convert latitudes from decimal degrees to radians
                decimal lat1Rad = DegreesToRadians(lat1);
                decimal lat2Rad = DegreesToRadians(lat2);

                // Haversine formula
                decimal a = (decimal)Math.Sin((double)(dLat / 2)) * (decimal)Math.Sin((double)(dLat / 2)) +
                            (decimal)Math.Cos((double)lat1Rad) * (decimal)Math.Cos((double)lat2Rad) *
                            (decimal)Math.Sin((double)(dLon / 2)) * (decimal)Math.Sin((double)(dLon / 2));

                decimal c = 2 * (decimal)Math.Atan2((double)Math.Sqrt((double)a), (double)Math.Sqrt(1 - (double)a));

                return EarthRadius * c;
            }
            catch (Exception ex)
            {
                // Use a more specific exception type if possible
                throw new InvalidOperationException("Error calculating distance.", ex);
            }
        }

        private static decimal DegreesToRadians(decimal degrees)
        {
            return degrees * (decimal)Math.PI / 180;
        }
    }
}
