using System.Data;
using System.Linq.Expressions;
using AutoMapper.Configuration;
using Sushi.Kql.Operators;
using Sushi.MediaKiwi.Services;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;

namespace Sushi.Kql;

/// <summary>
/// Provides methods to build a query which can be used by <see cref="KqlStatementGenerator"/> to generate a KQL statement.
/// </summary>
public class KustoQuery<T>
{
    public string Table { get; init; }
    public List<WhereCondition> WhereClause { get; } = [];
    public PagingData? Paging { get; private set; }
    public string? OrderBy { get; private set; }

    public KustoQuery(KustoDataMap<T> kustoMap)
    {
        if (kustoMap == null)
            throw new ArgumentNullException(nameof(kustoMap), $"No mapping defined for class {typeof(T)}. Add a nested class inheriting DataMap to {typeof(T)} or add a DataMap attribute to {typeof(T)}.");

        Map = kustoMap.DataMap;
        Table = kustoMap.TableName;
    }

    /// <summary>
    /// Gets an object representing the mapping between class T and database
    /// </summary>
    public DataMap Map { get; protected set; }

    public void Add(Expression<Func<T, object?>> mappingExpression, object? value)
    {
        Add(mappingExpression, value, ComparisonOperator.Equals);
    }

    public void Add(Expression<Func<T, object?>> mappingExpression, object? value, ComparisonOperator comparisonOperator)
    {
        var members = MicroORM.Supporting.ReflectionHelper.GetMemberTree(mappingExpression);

        var dataproperty = Map.Items.FirstOrDefault(x => x.MemberInfoTree.SequenceEqual(members));
        if (dataproperty == null)
            throw new ArgumentException($"Could not find member [{string.Join(".", members.Select(x => x.Name))}] for type {typeof(T)}", nameof(mappingExpression));

        Add(dataproperty.Column, dataproperty.SqlType, value, comparisonOperator);
    }

    public string FindColumn(Expression<Func<T, object?>> mappingExpression)
    {
        var members = MicroORM.Supporting.ReflectionHelper.GetMemberTree(mappingExpression);

        var dataproperty = Map.Items.FirstOrDefault(x => x.MemberInfoTree.SequenceEqual(members));
        if (dataproperty == null)
            throw new ArgumentException($"Could not find member [{string.Join(".", members.Select(x => x.Name))}] for type {typeof(T)}", nameof(mappingExpression));

        return dataproperty.Column;
    }

    private void Add(string column, SqlDbType type, object? value, ComparisonOperator comparisonOperator)
    {
        var where = new WhereCondition(column, type, value, comparisonOperator);
        WhereClause.Add(where);
    }

    public void AddKql(string customKql)
    {
        var where = new WhereCondition(customKql);
        WhereClause.Add(where);
    }

    public void AddPaging(SortValues<T> sorting, PagingValues paging)
    {
        AddOrder(sorting);
        Paging = new PagingData(paging.PageSize, paging.PageIndex);
    }

    public void AddOrder(SortValues<T> sorting)
    {
        var members = MicroORM.Supporting.ReflectionHelper.GetMemberTree(sorting.SortField);
        var dataproperty = Map.Items.FirstOrDefault(x => x.MemberInfoTree.SequenceEqual(members));
        if (dataproperty == null)
            throw new ArgumentException($"Could not find member [{string.Join(".", members.Select(x => x.Name))}] for type {typeof(T)}", nameof(sorting));

        OrderBy = $"{dataproperty.Column} {sorting.Direction.ToString().ToLower()}";
    }
}
