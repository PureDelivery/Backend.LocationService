using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.Core.Contracts
{
    public interface ILocationService
    {
        FilterRestaurantsByLocationResponse GetRestaurantsDeliveryDataMatchedForLocation(FilterRestaurantsByLocationRequest filterRestaurantsByLocationRequest);
    }
}
