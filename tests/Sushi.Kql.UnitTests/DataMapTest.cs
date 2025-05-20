namespace Sushi.Kql.UnitTests;
public class DataMapTest
{
    [Fact]
    public void DefaultColumnMapping_Names()
    {
        var map = new DefaultColumnMapping();

        Assert.Equal("Timestamp", map.GetItem(x => x.Timestamp).Column);
        Assert.Equal("Magnitude", map.GetItem(x => x.Magnitude).Column);
        Assert.Equal("DepthInMeters", map.GetItem(x => x.DepthInMeters).Column);
        Assert.Equal("Location_Latitude", map.GetItem(x => x.Location.Latitude).Column);
        Assert.Equal("Location_Longitude", map.GetItem(x => x.Location.Longitude).Column);

        var ingestionMappings = map.GetIngestionMapping();
        Assert.Equal("$.Timestamp", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Timestamp").Properties["Path"]);
        Assert.Equal("$.Magnitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Magnitude").Properties["Path"]);
        Assert.Equal("$.DepthInMeters", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "DepthInMeters").Properties["Path"]);
        Assert.Equal("$.Location.Latitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Location_Latitude").Properties["Path"]);
        Assert.Equal("$.Location.Longitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Location_Longitude").Properties["Path"]);
    }

    [Fact]
    public void CustomColumnMapping_Names()
    {
        var map = new CustomColumnMapping();

        Assert.Equal("Timestamp", map.GetItem(x => x.Timestamp).Column);
        Assert.Equal("Magnitude", map.GetItem(x => x.Magnitude).Column);
        Assert.Equal("Depth", map.GetItem(x => x.DepthInMeters).Column);
        Assert.Equal("Lat", map.GetItem(x => x.Location.Latitude).Column);
        Assert.Equal("Lng", map.GetItem(x => x.Location.Longitude).Column);

        var ingestionMappings = map.GetIngestionMapping();
        Assert.Equal(5, ingestionMappings.IngestionMappings.Count());
        Assert.Equal("$.Timestamp", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Timestamp").Properties["Path"]);
        Assert.Equal("$.Magnitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Magnitude").Properties["Path"]);
        Assert.Equal("$.DepthInMeters", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Depth").Properties["Path"]);
        Assert.Equal("$.Location.Latitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Lat").Properties["Path"]);
        Assert.Equal("$.Location.Longitude", ingestionMappings.IngestionMappings.Single(x => x.ColumnName == "Lng").Properties["Path"]);
    }

    [Fact]
    public void GetDataProperty_InvalidExpression_ThrowsArgumentException()
    {
        var map = new NoDefinedMappings();

        Assert.Throws<ArgumentException>(() => map.GetItem(x => x.Timestamp));
    }


    public class DefaultColumnMapping : DataMap<Earthquake>
    {
        public DefaultColumnMapping()
        {
            Table("Earthquakes");
            Map(x => x.Timestamp);
            Map(x => x.Magnitude);
            Map(x => x.DepthInMeters);
            Map(x => x.Location.Latitude);
            Map(x => x.Location.Longitude);
            
        }
    }

    public class CustomColumnMapping : DataMap<Earthquake>
    {
        public CustomColumnMapping()
        {
            Table("Earthquakes");
            Map(x => x.Timestamp, "Timestamp");
            Map(x => x.Magnitude, "Magnitude");
            Map(x => x.DepthInMeters, "Depth");
            Map(x => x.Location.Latitude, "Lat");
            Map(x => x.Location.Longitude, "Lng");
            Map(x => x.ReadOnlyLongitude, "Lng").ReadOnly();
        }
    }

    public class NoDefinedMappings : DataMap<Earthquake>
    {
        public NoDefinedMappings()
        {
            
        }
    }

}
