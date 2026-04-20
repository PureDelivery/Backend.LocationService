namespace PureDelivery.LocationService.Configuration;

public class CourierRadiusConfig
{
    public decimal OnFoot     { get; set; } = 1.5m;
    public decimal Bicycle    { get; set; } = 3.0m;
    public decimal Scooter    { get; set; } = 5.0m;
    public decimal Motorcycle { get; set; } = 8.0m;
    public decimal Car        { get; set; } = 12.0m;
}
