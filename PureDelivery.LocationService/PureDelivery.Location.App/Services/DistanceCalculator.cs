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

        public double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLng = DegreesToRadians(lng2 - lng1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;

            return Math.Round(distance, 2);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
