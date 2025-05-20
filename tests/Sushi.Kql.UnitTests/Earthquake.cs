namespace Sushi.Kql.UnitTests;
public class Earthquake
{
    public required DateTime Timestamp { get; set; }
    public required float Magnitude { get; set; }
    public required int DepthInMeters { get; set; }
    public required Location Location { get; set; }
    public required float ReadOnlyLongitude { get; set; }
}

public class Location
{
    public required float Latitude { get; set; }
    public required float Longitude { get; set; }
}
