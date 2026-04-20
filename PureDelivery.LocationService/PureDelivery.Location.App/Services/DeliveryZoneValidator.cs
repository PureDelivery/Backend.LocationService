using PureDelivery.Location.Core.Contracts;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.App.Services
{
    public class DeliveryZoneValidator : IDeliveryZoneValidator
    {
        public bool IsPointInDeliveryZone(double userLat, double userLng, List<ZonePointData> zonePoints)
        {
            if (zonePoints == null || zonePoints.Count < 3)
                return false;

            var orderedPoints = zonePoints.OrderBy(p => p.Order).ToList();
            return IsPointInPolygon(userLat, userLng, orderedPoints);
        }

        private bool IsPointInPolygon(double pointLat, double pointLng, List<ZonePointData> polygonPoints)
        {
            var polygon = polygonPoints.Select(p => new PointD(p.Latitude, p.Longitude)).ToArray();
            return IsPointInPolygon(pointLat, pointLng, polygon);
        }

        private bool IsPointInPolygon(double x, double y, PointD[] polygon)
        {
            int n = polygon.Length;
            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((polygon[i].Y > y) != (polygon[j].Y > y)) &&
                    (x < (polygon[j].X - polygon[i].X) * (y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private struct PointD
        {
            public double X { get; }
            public double Y { get; }

            public PointD(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
