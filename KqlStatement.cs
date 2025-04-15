namespace Sushi.Kql;

/// <summary>
/// Represents a KQL statement to be executed against an ADX database.
/// </summary>
public record KqlStatement
{
    /// <summary>
    /// Collection of <see cref="KqlParameter"/> objects describing the parameters used in the statement.
    /// </summary>
    public List<KqlParameter> Parameters { get; init; } = [];

    /// <summary>
    /// The KQL statement to be executed.
    /// </summary>
    public string Kql { get; init; } = null!;
    public Dictionary<string, string> ParameterDictionary => Parameters.ToDictionary(x => x.Name, x => x.Value);
}
