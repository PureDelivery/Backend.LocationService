using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.Core.Contracts
{
    public interface IDeliveryZoneValidator
    {
        bool IsPointInDeliveryZone(decimal userLat, decimal userLng, List<ZonePointData> zonePoints);
    }
}
