namespace Sushi.Kql.UnitTests.Fixture;
public class Earthquake
{
    public DateTime Timestamp { get; set; }
    public float Magnitude { get; set; }
    public int DepthInMeters { get; set; }
    public Location Location { get; set; } = null!;

}

public class Location
{
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}
