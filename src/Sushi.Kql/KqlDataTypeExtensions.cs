namespace Sushi.Kql;

/// <summary>
/// Adds extension methods to <see cref="KqlDataType"/>
/// </summary>
public static class KqlDataTypeExtensions
{
    /// <summary>
    /// Returns the KQL string representation of the <see cref="KqlDataType"/>.
    /// </summary>    
    public static string ToString(this KqlDataType type)
    {
        return type switch
        {
            KqlDataType.String => "string",
            KqlDataType.Int => "int",
            KqlDataType.Long => "long",
            KqlDataType.Real => "real",
            KqlDataType.Decimal => "decimal",
            KqlDataType.DateTime => "datetime",
            KqlDataType.Guid => "guid",
            KqlDataType.Boolean => "boolean",
            KqlDataType.TimeSpan => "timespan",
            KqlDataType.Dynamic => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
