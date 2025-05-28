namespace Sushi.Kql.TimeSeries;

/// <summary>
/// Defines options for the 'make-series' operator.
/// </summary>
public enum MakeSeriesKind
{
    /// <summary>
    /// If a bucket has no rows, the default value for the aggregate is used, e.g. 0 when count().
    /// </summary>
    NonEmpty = 1,
    /// <summary>
    /// If a bucket has no rows, it is not included in the result set.
    /// </summary>
    Empty = 2
}
