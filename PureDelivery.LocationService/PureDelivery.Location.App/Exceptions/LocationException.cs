using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.App.Exceptions
{
    public class LocationException : Exception
    {
        public string ErrorCode { get; }
        public string UserMessage { get; }

        public LocationException(
            string errorCode,
            string userMessage,
            string? internalMessage = null,
            Exception? innerException = null)
            : base(internalMessage ?? userMessage, innerException)
        {
            ErrorCode = errorCode;
            UserMessage = userMessage;
        }

        public static LocationException ValidationError(string userMessage, string? internalMessage = null) =>
            new("VALIDATION_ERROR", userMessage, internalMessage);

        public static LocationException NoRestaurants() =>
            new("NO_RESTAURANTS", "No restaurants provided for filtering");

        public static LocationException InvalidCoordinates() =>
            new("INVALID_COORDINATES", "Invalid latitude or longitude coordinates");

        public static LocationException ProcessingError(string internalMessage) =>
            new("PROCESSING_ERROR", "Error processing location data", internalMessage);

        public static LocationException InternalError(string internalMessage) =>
            new("INTERNAL_ERROR", "Location service is temporarily unavailable", internalMessage);
    }
}
