using Sushi.Kql.Mapping;

namespace Sushi.Kql.UnitTests;

public class ConversionTest
{
    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsInt()
    {
        var typeOfInt = typeof(int);
        Assert.Equal(KqlDataType.Int, Conversion.GetKqlDataType(typeOfInt));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnLong_WhenTypeIsLong()
    {
        var typeOfLong = typeof(long);
        Assert.Equal(KqlDataType.Long, Conversion.GetKqlDataType(typeOfLong));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnBoolean_WhenTypeIsBool()
    {
        var typeOfBool = typeof(bool);
        Assert.Equal(KqlDataType.Boolean, Conversion.GetKqlDataType(typeOfBool));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnGuid_WhenTypeIsGuid()
    {
        var typeOfGuid = typeof(Guid);
        Assert.Equal(KqlDataType.Guid, Conversion.GetKqlDataType(typeOfGuid));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnDateTime_WhenTypeIsDateTime()
    {
        var typeOfDateTime = typeof(DateTime);
        Assert.Equal(KqlDataType.DateTime, Conversion.GetKqlDataType(typeOfDateTime));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnReal_WhenTypeIsDouble()
    {
        var typeOfDouble = typeof(double);
        Assert.Equal(KqlDataType.Real, Conversion.GetKqlDataType(typeOfDouble));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnTimeSpan_WhenTypeIsTimeSpan()
    {
        var typeOfTimeSpan = typeof(TimeSpan);
        Assert.Equal(KqlDataType.TimeSpan, Conversion.GetKqlDataType(typeOfTimeSpan));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnDynamic_WhenTypeIsByteArray()
    {
        var typeOfByte = typeof(byte[]);
        Assert.Equal(KqlDataType.Dynamic, Conversion.GetKqlDataType(typeOfByte));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnString_WhenTypeIsUnknown()
    {
        var typeOfUnknown = typeof(object);
        Assert.Equal(KqlDataType.String, Conversion.GetKqlDataType(typeOfUnknown));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsNullableInt()
    {
        var typeOfNullableInt = typeof(int?);
        Assert.Equal(KqlDataType.Int, Conversion.GetKqlDataType(typeOfNullableInt));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnString_WhenTypeIsNullableUnknown()
    {
        string? nullableString = "";
        var typeOfNullableString = nullableString?.GetType()!;
        Assert.Equal(KqlDataType.String, Conversion.GetKqlDataType(typeOfNullableString));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsEnum()
    {

        var type = typeof(FooBar);
        Assert.Equal(KqlDataType.Int, Conversion.GetKqlDataType(type));
    }

    enum FooBar
    {
        Foo,
        Bar
    };
}
