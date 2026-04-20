using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Location.Core.Contracts
{
    public interface IDistanceCalculator
    {
        decimal CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2);
    }
}
