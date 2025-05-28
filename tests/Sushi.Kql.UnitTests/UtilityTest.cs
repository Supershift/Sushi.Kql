using Sushi.Kql.Mapping;

namespace Sushi.Kql.UnitTests;

public class UtilityTest
{
    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsInt()
    {
        var typeOfInt = typeof(int);
        Assert.Equal(KqlDataType.Int, Utility.GetKqlDataType(typeOfInt));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnLong_WhenTypeIsLong()
    {
        var typeOfLong = typeof(long);
        Assert.Equal(KqlDataType.Long, Utility.GetKqlDataType(typeOfLong));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnBoolean_WhenTypeIsBool()
    {
        var typeOfBool = typeof(bool);
        Assert.Equal(KqlDataType.Boolean, Utility.GetKqlDataType(typeOfBool));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnGuid_WhenTypeIsGuid()
    {
        var typeOfGuid = typeof(Guid);
        Assert.Equal(KqlDataType.Guid, Utility.GetKqlDataType(typeOfGuid));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnDateTime_WhenTypeIsDateTime()
    {
        var typeOfDateTime = typeof(DateTime);
        Assert.Equal(KqlDataType.DateTime, Utility.GetKqlDataType(typeOfDateTime));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnReal_WhenTypeIsDouble()
    {
        var typeOfDouble = typeof(double);
        Assert.Equal(KqlDataType.Real, Utility.GetKqlDataType(typeOfDouble));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnTimeSpan_WhenTypeIsTimeSpan()
    {
        var typeOfTimeSpan = typeof(TimeSpan);
        Assert.Equal(KqlDataType.TimeSpan, Utility.GetKqlDataType(typeOfTimeSpan));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnDynamic_WhenTypeIsByteArray()
    {
        var typeOfByte = typeof(byte[]);
        Assert.Equal(KqlDataType.Dynamic, Utility.GetKqlDataType(typeOfByte));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnString_WhenTypeIsUnknown()
    {
        var typeOfUnknown = typeof(object);
        Assert.Equal(KqlDataType.String, Utility.GetKqlDataType(typeOfUnknown));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsNullableInt()
    {
        var typeOfNullableInt = typeof(int?);
        Assert.Equal(KqlDataType.Int, Utility.GetKqlDataType(typeOfNullableInt));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnString_WhenTypeIsNullableUnknown()
    {
        string? nullableString = "";
        var typeOfNullableString = nullableString?.GetType()!;
        Assert.Equal(KqlDataType.String, Utility.GetKqlDataType(typeOfNullableString));
    }

    [Fact]
    public void GetKqlDataType_ShouldReturnInt_WhenTypeIsEnum()
    {

        var type = typeof(FooBar);
        Assert.Equal(KqlDataType.Int, Utility.GetKqlDataType(type));
    }

    enum FooBar
    {
        Foo,
        Bar
    };
}
