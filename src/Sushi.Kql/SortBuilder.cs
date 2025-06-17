using System.Linq.Expressions;
using System.Text;
using Sushi.Kql.Mapping;

namespace Sushi.Kql;

/// <summary>
/// Builds the sort part of a KQL query.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SortBuilder<T>
{
    private readonly DataMap<T> _map;
    private readonly StringBuilder _builder;
    private bool _isFirst;

    internal SortBuilder(DataMap<T> map, StringBuilder builder)
    {
        _map = map;
        _builder = builder;
        _isFirst = true;
        builder.Append("| sort by ");
    }

    /// <summary>
    /// Adds a column to sort the query by
    /// </summary>
    /// <param name="column">The column of T by which to sort.</param>
    /// <param name="sortDirection">asc sorts into ascending order, low to high. Default is desc, high to low.</param>
    /// <param name="nullsPlacement">nulls first will place the null values at the beginning and nulls last will place the null values at the end. Default for asc is nulls first. Default for desc is nulls last.</param>
    /// <returns></returns>
    public SortBuilder<T> By(Expression<Func<T, object?>> column, SortDirection? sortDirection = null, NullsPlacement? nullsPlacement = null)
    {
        if (_isFirst)
        {
            _isFirst = false;
        }
        else
        {
            _builder.Append(", ");
        }
        var dataMapItem = _map.GetItem(column);
        _builder.Append(dataMapItem.Column);

        if (sortDirection.HasValue)
        {
            _builder.Append(' ').Append(sortDirection.Value.ToString().ToLower());
        }

        if (nullsPlacement.HasValue)
        {
            if (nullsPlacement == NullsPlacement.NullsFirst)
            {
                _builder.Append(" nulls first");
            }
            else if (nullsPlacement == NullsPlacement.NullsLast)
            {
                _builder.Append(" nulls last");
            }
        }

        return this;
    }
}

/// <summary>
/// Specifies the placement of <c>null</c> and special values (e.g., NaN, infinity) in sorted data.
/// </summary>
public enum NullsPlacement
{
    /// <summary>
    /// Places <c>null</c>, NaN, and negative infinity before all other values.
    /// <para>Ascending order: null, NaN, -5, 0, 5</para>
    /// <para>Descending order: null, NaN, 5, 0, -5</para>
    /// </summary>
    NullsFirst,
    /// <summary>
    /// Places <c>null</c>, NaN, and positive infinity after all other values.
    /// <para>Ascending order: -5, 0, 5, NaN, null</para>
    /// <para>Descending order: 5, 0, -5, NaN, null</para>
    /// </summary>
    NullsLast
}
