using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.Core.Contracts
{
    public interface IDistanceCalculator
    {
        double CalculateDistance(double lat1, double lng1, double lat2, double lng2);
    }
}
