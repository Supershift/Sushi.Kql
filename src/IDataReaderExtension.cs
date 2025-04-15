using System.Data;

namespace Sushi.Kql;

/// <summary>
/// Provides extension methods for <see cref="IDataReader"/> to simplify access for dynamically typed data.
/// </summary>
public static class IDataReaderExtension
{
    /// <summary>
    /// Return the index of the named field.
    /// </summary>
    /// <returns>The zero-based index of the named field if found, otherwise -1</returns>
    public static int GetOrdinalSafe(this IDataReader reader, string columnName)
    {
        try
        {
            return reader.GetOrdinal(columnName);
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Get a nullable string from the reader. Auto
    /// </summary>
    /// <returns>
    /// A string representation of the column value if it exists; otherwise, <c>null</c>
    /// </returns>
    public static string? GetNullableString(this IDataReader reader, string columnName)
    {
        // determine index
        var groupingIndex = reader.GetOrdinalSafe(columnName);
        if (groupingIndex == -1)
            return null;

        // Check if the value is DBNull first
        if (reader.IsDBNull(groupingIndex))
            return null;

        // get value
        if (reader.GetFieldType(groupingIndex) == typeof(Guid))
        {
            return reader.GetGuid(groupingIndex).ToString();
        }
        else if (reader.GetFieldType(groupingIndex) == typeof(DateTime))
        {
            return reader.GetDateTime(groupingIndex).ToString("O");
        }
        return reader.GetString(groupingIndex);
    }
}
