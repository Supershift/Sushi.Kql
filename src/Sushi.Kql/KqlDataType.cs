namespace Sushi.Kql;
/// <summary>
/// Defines KQL data types.
/// </summary>
public enum KqlDataType
{
    /// <summary>
    /// A sequence of zero or more Unicode characters.
    /// </summary>
    String,
    /// <summary>
    /// A signed, 32-bit wide, integer.
    /// </summary>
    Int,
    /// <summary>
    /// A signed, 64-bit wide, integer.
    /// </summary>
    Long,
    /// <summary>
    /// A 64-bit wide, double-precision, floating-point number.
    /// </summary>
    Real,
    /// <summary>
    /// A 128-bit wide, decimal number.
    /// </summary>
    Decimal,
    /// <summary>
    /// An instant in time, typically expressed as a date and time of day.
    /// </summary>
    DateTime,
    /// <summary>
    /// A 128-bit globally unique value.
    /// </summary>
    Guid,
    /// <summary>
    /// An array, a property bag, or a value of any of the other scalar data types.
    /// </summary>
    Dynamic,
    /// <summary>
    /// true (1) or false (0).
    /// </summary>
    Boolean,
    /// <summary>
    /// A time interval.
    /// </summary>
    TimeSpan,
}
