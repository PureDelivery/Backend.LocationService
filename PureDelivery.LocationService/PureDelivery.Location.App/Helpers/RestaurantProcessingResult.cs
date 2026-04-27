using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.App.Helpers
{
    public class RestaurantProcessingResult
    {
        public Guid RestaurantId { get; private set; }
        public bool IsDeliverable { get; private set; }
        public double Distance { get; private set; }
        public DeliveryZoneMatch? BestMatchingZone { get; private set; }

        private RestaurantProcessingResult(
            Guid restaurantId,
            bool isDeliverable,
            double distance,
            DeliveryZoneMatch? bestZone)
        {
            RestaurantId = restaurantId;
            IsDeliverable = isDeliverable;
            Distance = distance;
            BestMatchingZone = bestZone;
        }

        public static RestaurantProcessingResult Deliverable(
            Guid restaurantId,
            double distance,
            DeliveryZoneMatch bestZone)
        {
            return new RestaurantProcessingResult(restaurantId, true, distance, bestZone);
        }

        public static RestaurantProcessingResult NotDeliverable(Guid restaurantId)
        {
            return new RestaurantProcessingResult(restaurantId, false, 0, null);
        }
    }
}
