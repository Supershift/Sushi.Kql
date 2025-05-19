namespace Sushi.Kql.TestData;
public record Earthquake
{
    public required DateTime TimeStamp { get; set; }
    public required double Magnitude { get; set; }
    public required int DepthInMeters { get; set; }
    public required Location Location { get; set; }

}

public record Location
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}
