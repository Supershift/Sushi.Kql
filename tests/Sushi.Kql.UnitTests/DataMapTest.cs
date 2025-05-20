namespace Sushi.Kql.UnitTests;
public class DataMapTest
{
    [Fact]
    public void DefaultColumnMapping_Names()
    {
        var map = new DefaultColumnMapping();

        Assert.Equal("Timestamp", map.GetDataProperty(x => x.Timestamp).Column);
        Assert.Equal("Magnitude", map.GetDataProperty(x => x.Magnitude).Column);
        Assert.Equal("DepthInMeters", map.GetDataProperty(x => x.DepthInMeters).Column);
        Assert.Equal("Location_Latitude", map.GetDataProperty(x => x.Location.Latitude).Column);
        Assert.Equal("Location_Longitude", map.GetDataProperty(x => x.Location.Longitude).Column);

        var ingestionMappings = map.IngestionMapping.IngestionMappings;
        Assert.Equal("$.Timestamp", ingestionMappings.Single(x => x.ColumnName == "Timestamp").Properties["Path"]);
        Assert.Equal("$.Magnitude", ingestionMappings.Single(x => x.ColumnName == "Magnitude").Properties["Path"]);
        Assert.Equal("$.DepthInMeters", ingestionMappings.Single(x => x.ColumnName == "DepthInMeters").Properties["Path"]);
        Assert.Equal("$.Location.Latitude", ingestionMappings.Single(x => x.ColumnName == "Location_Latitude").Properties["Path"]);
        Assert.Equal("$.Location.Longitude", ingestionMappings.Single(x => x.ColumnName == "Location_Longitude").Properties["Path"]);
    }

    [Fact]
    public void CustomColumnMapping_Names()
    {
        var map = new CustomColumnMapping();

        Assert.Equal("Timestamp", map.GetDataProperty(x => x.Timestamp).Column);
        Assert.Equal("Magnitude", map.GetDataProperty(x => x.Magnitude).Column);
        Assert.Equal("Depth", map.GetDataProperty(x => x.DepthInMeters).Column);
        Assert.Equal("Lat", map.GetDataProperty(x => x.Location.Latitude).Column);
        Assert.Equal("Lng", map.GetDataProperty(x => x.Location.Longitude).Column);

        var ingestionMappings = map.IngestionMapping.IngestionMappings;
        Assert.Equal("$.Timestamp", ingestionMappings.Single(x => x.ColumnName == "Timestamp").Properties["Path"]);
        Assert.Equal("$.Magnitude", ingestionMappings.Single(x => x.ColumnName == "Magnitude").Properties["Path"]);
        Assert.Equal("$.DepthInMeters", ingestionMappings.Single(x => x.ColumnName == "Depth").Properties["Path"]);
        Assert.Equal("$.Location.Latitude", ingestionMappings.Single(x => x.ColumnName == "Lat").Properties["Path"]);
        Assert.Equal("$.Location.Longitude", ingestionMappings.Single(x => x.ColumnName == "Lng").Properties["Path"]);
    }

    [Fact]
    public void GetDataProperty_InvalidExpression_ThrowsArgumentException()
    {
        var map = new NoDefinedMappings();

        Assert.Throws<ArgumentException>(() => map.GetDataProperty(x => x.Timestamp));
    }


    public class DefaultColumnMapping : DataMap<Earthquake>
    {
        public override string TableName => "Earthquakes";

        public DefaultColumnMapping()
        {
            Map(x => x.Timestamp);
            Map(x => x.Magnitude);
            Map(x => x.DepthInMeters);
            Map(x => x.Location.Latitude);
            Map(x => x.Location.Longitude);
        }
    }

    public class CustomColumnMapping : DataMap<Earthquake>
    {
        public override string TableName => "Earthquakes";

        public CustomColumnMapping()
        {
            Map(x => x.Timestamp, "Timestamp");
            Map(x => x.Magnitude, "Magnitude");
            Map(x => x.DepthInMeters, "Depth");
            Map(x => x.Location.Latitude, "Lat");
            Map(x => x.Location.Longitude, "Lng");
        }
    }

    public class NoDefinedMappings : DataMap<Earthquake>
    {
        public override string TableName => "Earthquakes";
    }

}
