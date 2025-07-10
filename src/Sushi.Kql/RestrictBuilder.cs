using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Cloud.Platform.Utils;
using Sushi.Kql.Mapping;

namespace Sushi.Kql;
/// <summary>
/// Builds a restrict statement, which can be used to restrict access to certain tables or views in KQL queries.
/// </summary>
public class RestrictBuilder
{   
    private readonly List<string> _restrictedResources = [];
    private StringBuilder _builder = new StringBuilder();

    public ParameterCollection Parameters { get; } = new ParameterCollection("r");

    /// <summary>
    /// Adds a table to the list of restricted resources.
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public RestrictBuilder AddTable(string tableName)
    {
        _restrictedResources.Add($"databases().{tableName}");
        return this;
    }

    /// <summary>
    /// Adds a statement to the list of restricted resources.
    /// </summary>        
    public RestrictBuilder AddStatement(string statement)
    {
        _restrictedResources.Add(statement);
        return this;
    }

    /// <summary>
    /// Adds a KQL statement to the restrict query. Use <see cref="Parameters"/> to add any parameters.
    /// </summary>
    /// <param name="kql"></param>
    /// <returns></returns>
    public RestrictBuilder AddKql(string kql)
    {
        _builder.AppendLine(kql);
        return this;
    }

    /// <summary>
    /// Gets the KQL for this restrict statement, including parameter declaration.
    /// </summary>
    /// <returns></returns>
    public string ToKql()
    {
        // add parameters at top of query
        if (Parameters.Count > 0)
        {
            var parameters = Parameters.GetParameters();
            var stringified = string.Join(", ", parameters.Select(x => $"{x.Name}:{x.Type}"));
            _builder.Insert(0, $"declare query_parameters({stringified});{Environment.NewLine}");
        }

        // the stringbuilder already contains view definitions

        // add list of restricted resources to the bottom of the query
        _builder.AppendLine($"restrict access to ({string.Join(',', _restrictedResources)});");

        return _builder.ToString();
    }

    /// <summary>
    /// Adds a view to the list of restricted resources. The view can be defined using the <paramref name="viewBuilder"/>.
    /// </summary>    
    /// <param name="viewName"></param>
    /// <param name="viewBuilder"></param>
    /// <param name="map"></param>
    /// <param name="tableName">Which table will be used to build the view. If null, the default tablename for <typeparamref name="T"/> will be used.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RestrictBuilder AddView<T>(string viewName, Action<QueryBuilder<T>> viewBuilder, DataMap<T> map, string? tableName = null)
    {
        // determine tablename to use for the view's query        
        tableName ??= map.TableName;

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentNullException(nameof(tableName), $"No table name provided and mapping for type '{typeof(T)}' does not define a table name.");
        }

        AddStatement(viewName);

        _builder.Append($"let {viewName} = view ()");
        _builder.Append(" { ");

        // let the viewbuilder callback create the query for the view        
        var queryBuilder = new QueryBuilder<T>(map, _builder, Parameters, tableName);
        viewBuilder(queryBuilder);

        _builder.AppendLine(" };");

        return this;
    }
}

public class RestrictBuilder<T> : RestrictBuilder
{
    private readonly DataMap<T> _map;

    public RestrictBuilder(DataMap<T> map)
    {
        _map = map;
    }

    /// <summary>
    /// Adds a view to the list of restricted resources. The view can be defined using the <paramref name="viewBuilder"/>.
    /// </summary>    
    /// <param name="viewName"></param>
    /// <param name="viewBuilder"></param>
    /// <param name="tableName">Which table will be used to build the view. If null, the default tablename for <typeparamref name="T"/> will be used.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RestrictBuilder<T> AddView(string viewName, Action<QueryBuilder<T>> viewBuilder, string? tableName = null)
    {
        AddView<T>(viewName, viewBuilder, _map, tableName);
        return this;
    }
}
