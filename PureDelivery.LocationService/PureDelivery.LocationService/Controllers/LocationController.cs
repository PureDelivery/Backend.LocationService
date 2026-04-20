using Microsoft.AspNetCore.Mvc;
using PureDelivery.Location.App.Exceptions;
using PureDelivery.Location.Core.Contracts;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System.ComponentModel.DataAnnotations;

namespace PureDelivery.Location.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IDeliveryZoneValidator _zoneValidator;
        private readonly ILogger<LocationController> _logger;

        public LocationController(
            ILocationService locationService,
            IDeliveryZoneValidator zoneValidator,
            ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _zoneValidator = zoneValidator;
            _logger = logger;
        }

        [HttpPost("check-point-in-zone")]
        public ActionResult<BaseResponse<CheckPointInZoneResponse>> CheckPointInZone(
            [FromBody] CheckPointInZoneRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(BaseResponse<CheckPointInZoneResponse>.Failure("Invalid request data"));
                }

                var isInZone = _zoneValidator.IsPointInDeliveryZone(
                    request.UserLatitude,
                    request.UserLongitude,
                    request.ZonePoints);

                var response = new CheckPointInZoneResponse
                {
                    IsInZone = isInZone,
                    UserLatitude = request.UserLatitude,
                    UserLongitude = request.UserLongitude,
                    ZoneName = request.ZoneName
                };

                return Ok(BaseResponse<CheckPointInZoneResponse>.Success(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking point in zone");
                return StatusCode(500, BaseResponse<CheckPointInZoneResponse>.Failure("Internal server error"));
            }
        }


        // TODO: move to helper?
        // TODO: create shared error codes to handle from rest service?   ErrorCode



        [HttpPost("matched-location-restaurants")]
        public ActionResult<BaseResponse<FilterRestaurantsByLocationResponse>> GetRestaurantsDeliveryDataMatchedForLocation(
                   [FromBody] FilterRestaurantsByLocationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(BaseResponse<FilterRestaurantsByLocationResponse>.Failure("Invalid request data"));
                }

                var response = _locationService.GetRestaurantsDeliveryDataMatchedForLocation(request);

                return Ok(BaseResponse<FilterRestaurantsByLocationResponse>.Success(response));
            }
            catch (LocationException ex)
            {
                _logger.LogWarning(ex, "Location service error: {ErrorCode}", ex.ErrorCode);

                var statusCode = ex.ErrorCode switch
                {
                    "VALIDATION_ERROR" or "NO_RESTAURANTS" or "INVALID_COORDINATES" => 400,
                    _ => 200
                };

                return StatusCode(statusCode, BaseResponse<FilterRestaurantsByLocationResponse>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in location service");
                return StatusCode(500, BaseResponse<FilterRestaurantsByLocationResponse>.Failure("Internal server error"));
            }
        }
    }


    public class CheckPointInZoneRequest
    {
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal UserLatitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal UserLongitude { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Zone must have at least 3 points")]
        public List<ZonePointData> ZonePoints { get; set; } = new();

        public string ZoneName { get; set; } = "Test Zone";
    }

    public class CheckPointInZoneResponse
    {
        public bool IsInZone { get; set; }
        public decimal UserLatitude { get; set; }
        public decimal UserLongitude { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string Message => IsInZone
            ? $"Point ({UserLatitude}, {UserLongitude}) is inside {ZoneName}"
            : $"Point ({UserLatitude}, {UserLongitude}) is outside {ZoneName}";
    }
}