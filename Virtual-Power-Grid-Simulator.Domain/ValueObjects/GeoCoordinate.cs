using System;

namespace Virtual_Power_Grid_Simulator.Domain.ValueObjects;

public class GeoCoordinate
{
    public decimal Latitude {get; }
    public decimal Longitude {get; }

    public GeoCoordinate(decimal longitude, decimal latitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));
        
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }
}
