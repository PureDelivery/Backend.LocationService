namespace PureDelivery.LocationService.Configuration;

public class CourierRadiusConfig
{
    public double OnFoot     { get; set; } = 1.5;
    public double Bicycle    { get; set; } = 3.0;
    public double Scooter    { get; set; } = 5.0;
    public double Motorcycle { get; set; } = 8.0;
    public double Car        { get; set; } = 12.0;
}
