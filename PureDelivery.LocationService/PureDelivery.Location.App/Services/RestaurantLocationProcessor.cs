using Microsoft.Extensions.Logging;
using PureDelivery.Location.App.Exceptions;
using PureDelivery.Location.App.Helpers;
using PureDelivery.Location.Core.Contracts;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.App.Services
{
    public class RestaurantLocationProcessor
    {
        private readonly IDeliveryZoneValidator _zoneValidator;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly ILogger<RestaurantLocationProcessor> _logger;

        public RestaurantLocationProcessor(
            IDeliveryZoneValidator zoneValidator,
            IDistanceCalculator distanceCalculator,
            ILogger<RestaurantLocationProcessor> logger)
        {
            _zoneValidator = zoneValidator;
            _distanceCalculator = distanceCalculator;
            _logger = logger;
        }

        public RestaurantProcessingResult ProcessRestaurant(
            RestaurantLocationData restaurant,
            decimal userLat,
            decimal userLng)
        {
            try
            {
                ValidateRestaurantData(restaurant);
                ValidateCoordinates(userLat, userLng);

                var matchingZone = FindMatchingDeliveryZone(restaurant, userLat, userLng);

                if (matchingZone == null)
                {
                    return RestaurantProcessingResult.NotDeliverable(restaurant.RestaurantId);
                }

                var distance = CalculateDistanceToRestaurant(restaurant, userLat, userLng);
                var zoneMatch = MapToDeliveryZoneMatch(matchingZone);

                return RestaurantProcessingResult.Deliverable(
                    restaurant.RestaurantId,
                    distance,
                    zoneMatch);
            }
            catch (LocationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing restaurant {RestaurantId}",
                    restaurant?.RestaurantId);
                throw LocationException.ProcessingError(
                    $"Failed to process restaurant {restaurant?.RestaurantId}: {ex.Message}");
            }
        }

        private void ValidateRestaurantData(RestaurantLocationData restaurant)
        {
            if (restaurant == null)
            {
                throw LocationException.ValidationError("Restaurant data cannot be null");
            }

            if (restaurant.RestaurantId == Guid.Empty)
            {
                throw LocationException.ValidationError("Restaurant ID cannot be empty");
            }

            if (restaurant.DeliveryZones == null || !restaurant.DeliveryZones.Any())
            {
                _logger.LogWarning("Restaurant {RestaurantId} has no delivery zones", restaurant.RestaurantId);
                return;
            }

            if (restaurant.Latitude < -90 || restaurant.Latitude > 90)
            {
                throw LocationException.ValidationError(
                    $"Invalid restaurant latitude: {restaurant.Latitude}");
            }

            if (restaurant.Longitude < -180 || restaurant.Longitude > 180)
            {
                throw LocationException.ValidationError(
                    $"Invalid restaurant longitude: {restaurant.Longitude}");
            }
        }

        private void ValidateCoordinates(decimal userLat, decimal userLng)
        {
            if (userLat < -90 || userLat > 90)
            {
                throw LocationException.InvalidCoordinates();
            }

            if (userLng < -180 || userLng > 180)
            {
                throw LocationException.InvalidCoordinates();
            }
        }

        private DeliveryZoneData? FindMatchingDeliveryZone(
            RestaurantLocationData restaurant,
            decimal userLat,
            decimal userLng)
        {
            return restaurant.DeliveryZones
                    .Where(zone => zone.IsActive)
                    .Where(zone => _zoneValidator.IsPointInDeliveryZone(userLat, userLng, zone.Points))
                    .OrderBy(zone => zone.Priority)
                    .FirstOrDefault();
        }

        private decimal CalculateDistanceToRestaurant(
            RestaurantLocationData restaurant,
            decimal userLat,
            decimal userLng)
        {
            return _distanceCalculator.CalculateDistance(
                userLat, userLng,
                restaurant.Latitude, restaurant.Longitude);
        }

        private DeliveryZoneMatch MapToDeliveryZoneMatch(DeliveryZoneData zone)
        {
            return new DeliveryZoneMatch
            {
                ZoneId = zone.ZoneId,
                Name = zone.Name,
                DeliveryFee = zone.DeliveryFee,
                MinOrderAmount = zone.MinOrderAmount,
                EstimatedDeliveryMinutes = zone.EstimatedDeliveryMinutes,
                Priority = zone.Priority
            };
        }
    }
}