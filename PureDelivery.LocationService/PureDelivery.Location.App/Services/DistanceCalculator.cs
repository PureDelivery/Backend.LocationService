using PureDelivery.Location.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.App.Services
{
    public class DistanceCalculator : IDistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0;

        public decimal CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
        {
            var dLat = DegreesToRadians((double)(lat2 - lat1));
            var dLng = DegreesToRadians((double)(lng2 - lng1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians((double)lat1)) * Math.Cos(DegreesToRadians((double)lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;

            return (decimal)Math.Round(distance, 2);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
