using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.Mapping;

namespace Sushi.Kql.Tabular;
/// <summary>
/// Builds a KQL mv-expand operation with a fluent API.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MvExpandBuilder<T>
{
    private bool _isFirst;
    private readonly StringBuilder _builder;
    private readonly DataMap<T> _map;

    internal MvExpandBuilder(StringBuilder builder, DataMap<T> map)
    {
        _isFirst = true;
        _builder = builder;
        _map = map;
        builder.Append("| mv-expand ");
    }

    /// <summary>
    /// Adds an expression to the mv-expand operation.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="kqlDataType"></param>
    /// <returns></returns>
    public MvExpandBuilder<T> Add(string expression, KqlDataType kqlDataType)
    {
        if (_isFirst)
        {
            _isFirst = false;
        }
        else
        {
            _builder.Append(',');
        }

        _builder.Append($"{expression} to typeof({kqlDataType.ToKqlString()})");

        return this;
    }

    /// <summary>
    /// Adds an expression to the mv-expand operation.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public MvExpandBuilder<T> Add(Expression<Func<T, object?>> expression)
    {
        var mapItem = _map.GetItem(expression);
        return Add(mapItem.Column, mapItem.DataType);
    }
}
