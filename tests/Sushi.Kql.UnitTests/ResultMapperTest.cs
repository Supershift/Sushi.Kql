using System.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Sushi.Kql.UnitTests.Mocks;

namespace Sushi.Kql.UnitTests;
public class ResultMapperTest
{
    [Fact]
    public void MapToMultipleResults_ShouldReturnMappedList()
    {
        var dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        var earthquake = new Earthquake()
        {
            Timestamp = DateTime.ParseExact("2025-04-15T11:22:33Z", dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None),
            Magnitude = 5.1f,
            DepthInMeters = 10000,
            Location = new Location()
            {
                Latitude = 16.772f,
                Longitude = 23.430f
            }
        };
        var earthquake2 = new Earthquake()
        {
            Timestamp = DateTime.ParseExact("2025-04-15T10:54:20Z", dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None),
            Magnitude = 2.7f,
            DepthInMeters = 32900,
            Location = new Location()
            {
                Latitude = 19.276f,
                Longitude = 155.399f
            }
        };


        // Arrange
        var rows = new List<Dictionary<string, object?>>
        {
            new() { { "Timestamp", earthquake.Timestamp }, { "Magnitude", earthquake.Magnitude }, { "DepthInMeters", earthquake.DepthInMeters }, { "Latitude", earthquake.Location.Latitude }, { "Longitude", earthquake.Location.Longitude } },
            new() { { "Timestamp", earthquake2.Timestamp }, { "Magnitude", earthquake2.Magnitude }, { "DepthInMeters", earthquake2.DepthInMeters }, { "Latitude", earthquake2.Location.Latitude }, { "Longitude", earthquake2.Location.Longitude } },
        };
        var mockReader = new MockDataReader(rows);
        var dataMap = new EarthquakeMap();


        // Act
        var result = ResultMapper.MapToMultipleResults(mockReader, dataMap);

        // Assert size and row ids
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(earthquake.Timestamp, result[0].Timestamp);
        Assert.Equal(earthquake2.Timestamp, result[1].Timestamp);
    }

    [Fact]
    public void MapRowToObject_ShouldMapValuesCorrectly()
    {
        // Arrange
        var dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        var earthquake = new Earthquake()
        {
            Timestamp = DateTime.ParseExact("2025-04-15T11:22:33Z", dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None),
            Magnitude = 5.1f,
            DepthInMeters = 10000,
            Location = new Location()
            {
                Latitude = 16.772f,
                Longitude = 23.430f
            }
        };
        var rows = new List<Dictionary<string, object?>>
        {
            new() { { "Timestamp", earthquake.Timestamp }, { "Magnitude", earthquake.Magnitude }, { "DepthInMeters", earthquake.DepthInMeters }, { "Location_Latitude", earthquake.Location.Latitude }, { "Location_Longitude", earthquake.Location.Longitude } },
        };
        var mockReader = new MockDataReader(rows);
        var dataMap = new EarthquakeMap();

        var instance = (Earthquake)Activator.CreateInstance(typeof(Earthquake), true)!;

        // Act
        mockReader.Read(); // load first row for mapping
        ResultMapper.MapRowToObject(mockReader, dataMap, instance);

        // Assert
        Assert.Equal(earthquake.Timestamp, instance.Timestamp);
        Assert.Equal(earthquake.Magnitude, instance.Magnitude);
        Assert.Equal(earthquake.DepthInMeters, instance.DepthInMeters);
        Assert.NotNull(instance.Location);
        Assert.Equal(earthquake.Location.Latitude, instance.Location.Latitude);
        Assert.Equal(earthquake.Location.Longitude, instance.Location.Longitude);
    }

    [Fact]
    public void MapRowToObject_ShouldMapJTokenToType()
    {
        // Arrange
        var dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        var earthquake = new Earthquake()
        {
            Timestamp = DateTime.ParseExact("2025-04-15T11:22:33Z", dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None),
            Magnitude = 5.1f,
            DepthInMeters = 10000,
            Location = new Location()
            {
                Latitude = 16.772f,
                Longitude = 23.430f
            }
        };
        var location = JToken.FromObject(earthquake.Location);

        var rows = new List<Dictionary<string, object?>>
        {
            new() { { "Timestamp", earthquake.Timestamp }, { "Magnitude", earthquake.Magnitude }, { "DepthInMeters", earthquake.DepthInMeters }, { "Location", location } },
        };
        var mockReader = new MockDataReader(rows);
        var dataMap = new EarthquakeWithDynamicLocationMap();

        var instance = (Earthquake)Activator.CreateInstance(typeof(Earthquake), true)!;

        // Act
        mockReader.Read(); // load first row for mapping
        ResultMapper.MapRowToObject(mockReader, dataMap, instance);

        // Assert
        Assert.Equal(earthquake.Timestamp, instance.Timestamp);
        Assert.Equal(earthquake.Magnitude, instance.Magnitude);
        Assert.Equal(earthquake.DepthInMeters, instance.DepthInMeters);
        Assert.Equal(earthquake.Location.Latitude, instance.Location.Latitude);
        Assert.Equal(earthquake.Location.Longitude, instance.Location.Longitude);
    }

    [Fact]
    public void MapRowToObject_ShouldThrowArgumentNullException_WhenInstanceIsNull()
    {
        // Arrange
        var mockReader = new MockDataReader([]);
        var dataMap = new EarthquakeMap();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ResultMapper.MapRowToObject(mockReader, dataMap!, null));
    }

    public class EarthquakeMap : DataMap<Earthquake>
    {
        public override string TableName => "Earthquakes";

        public EarthquakeMap()
        {
            Map(x => x.Timestamp);
            Map(x => x.Magnitude);
            Map(x => x.DepthInMeters);
            Map(x => x.Location.Latitude);
            Map(x => x.Location.Longitude);
        }
    }

    public class EarthquakeWithDynamicLocationMap : DataMap<Earthquake>
    {
        public override string TableName => "Earthquakes";

        public EarthquakeWithDynamicLocationMap()
        {
            Map(x => x.Timestamp);
            Map(x => x.Magnitude);
            Map(x => x.DepthInMeters);
            Map(x => x.Location);
        }
    }

}



