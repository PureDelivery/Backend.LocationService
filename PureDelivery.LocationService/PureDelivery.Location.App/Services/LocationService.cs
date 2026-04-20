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
    public class LocationService : ILocationService
    {
        private readonly RestaurantLocationProcessor _processor;
        private readonly ILogger<LocationService> _logger;

        public LocationService(RestaurantLocationProcessor processor,
            ILogger<LocationService> logger)
        {
            _processor = processor;
            _logger = logger;
        }







        // TODO: add caching for same location requests!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // TODO: ADD LOGGING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!





        public FilterRestaurantsByLocationResponse GetRestaurantsDeliveryDataMatchedForLocation(
                    FilterRestaurantsByLocationRequest request)
        {
            try
            {
                _logger.LogInformation("Processing location request for {RestaurantCount} restaurants at ({Lat}, {Lng})",
                    request.Restaurants?.Count ?? 0, request.UserLatitude, request.UserLongitude);

                ValidateRequest(request);
                var results = ProcessRestaurants(request);
                var response = BuildResponse(results);

                _logger.LogInformation("Successfully processed {DeliverableCount} deliverable restaurants",
                    response.DeliverableRestaurants.Count);

                return response;
            }
            catch (LocationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing location request");
                throw LocationException.InternalError($"Unexpected error: {ex.Message}");
            }
        }

        private void ValidateRequest(FilterRestaurantsByLocationRequest request)
        {
            if (request == null)
            {
                throw LocationException.ValidationError("Request cannot be null");
            }

            if (request.Restaurants == null || !request.Restaurants.Any())
            {
                throw LocationException.NoRestaurants();
            }

            if (request.UserLatitude < -90 || request.UserLatitude > 90)
            {
                throw LocationException.InvalidCoordinates();
            }

            if (request.UserLongitude < -180 || request.UserLongitude > 180)
            {
                throw LocationException.InvalidCoordinates();
            }
        }

        private List<RestaurantProcessingResult> ProcessRestaurants(FilterRestaurantsByLocationRequest request)
        {
            try
            {
                return request.Restaurants
                    .Select(restaurant => _processor.ProcessRestaurant(
                        restaurant,
                        request.UserLatitude,
                        request.UserLongitude))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing restaurants");
                throw LocationException.ProcessingError($"Failed to process restaurants: {ex.Message}");
            }
        }

        private FilterRestaurantsByLocationResponse BuildResponse(List<RestaurantProcessingResult> results)
        {
            var deliverableRestaurants = results
                .Where(r => r.IsDeliverable)
                .Select(r => new DeliverableRestaurant
                {
                    RestaurantId = r.RestaurantId,
                    Distance = r.Distance,
                    BestDeliveryZone = r.BestMatchingZone!
                })
                .ToList();

            var nonDeliverableIds = results
                .Where(r => !r.IsDeliverable)
                .Select(r => r.RestaurantId)
                .ToList();

            return new FilterRestaurantsByLocationResponse
            {
                DeliverableRestaurants = deliverableRestaurants,
                NonDeliverableRestaurantIds = nonDeliverableIds
            };
        }
    }
}
