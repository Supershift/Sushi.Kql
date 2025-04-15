using System.Collections;
using System.Data;
using System.Text;
using Sushi.MicroORM;

namespace Sushi.Kql;

/// <summary>
/// Provides methods to generate instance of <see cref="KqlStatement"/>.
/// </summary>
public class KqlStatementGenerator
{
    /// <summary>
    /// Generates a <see cref="KqlStatement"/> from a <see cref="KustoQuery{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public KqlStatement GenerateKqlQuery<T>(KustoQuery<T> query)
    {
        var parameters = new List<KqlParameter>();
        // Get data
        var sb = new StringBuilder(query.Table).AppendLine();
        // Filter
        AppendWhere(query, sb, parameters);
        // Summarize

        // Sort
        AppendOrder(query, sb);
        // Select
        AppendPaging(query, sb);

        InsertParameters(sb, parameters);
        return new KqlStatement() { Kql = sb.ToString(), Parameters = parameters };
    }



    private static void AppendPaging<T>(KustoQuery<T> query, StringBuilder sb)
    {
        if (query.Paging == null)
            return;
        // should be optimized by caching query results in the future https://learn.microsoft.com/en-us/kusto/query/stored-query-result-function?view=azure-data-explorer
        var skippedRows = query.Paging.PageIndex * query.Paging.NumberOfRows;
        var fromRow = skippedRows + 1; // first row is 1
        var toRow = skippedRows + query.Paging.NumberOfRows;
        sb.AppendLine($" | extend RowNumber = row_number()");
        sb.AppendLine($" | where RowNumber between({fromRow}..{toRow});");

        // add query to count total rows
        var countSb = new StringBuilder(query.Table);
        AppendWhere(query, countSb, new List<KqlParameter>());
        countSb.AppendLine(" | count;");
        sb.AppendLine(countSb.ToString());
    }

    private static void AppendOrder<T>(KustoQuery<T> query, StringBuilder sb)
    {
        if (string.IsNullOrWhiteSpace(query.OrderBy))
            return;
        sb.AppendLine($" | order by {query.OrderBy}");
    }

    private static void InsertParameters(StringBuilder sb, List<KqlParameter> parameters)
    {
        var stringified = string.Join(", ", parameters.Select(x => $"{x.Name}:{x.Type}"));
        sb.Insert(0, $"declare query_parameters({stringified});\n");
    }

    private static void AppendWhere<T>(KustoQuery<T> query, StringBuilder sb, List<KqlParameter> parameters)
    {
        if (!query.WhereClause.Any())
        {
            return;
        }

        for (int i = 0; i < query.WhereClause.Count; i++)
        {
            var where = query.WhereClause[i];
            string parameterName = $"P{i}";

            if (!string.IsNullOrEmpty(where.SqlText))
            {
                sb.AppendLine($" | where {where.SqlText}");
            }
            else if (where.CompareType == ComparisonOperator.Equals)
            {
                sb.AppendLine($" | where {where.Column} == {parameterName}");
                parameters.Add(new KqlParameter(where.SqlType, parameterName, where.Value));
            }
            else if (where.CompareType == ComparisonOperator.GreaterThanOrEquals)
            {
                sb.AppendLine($" | where {where.Column} >= {parameterName}");
                parameters.Add(new KqlParameter(where.SqlType, parameterName, where.Value));
            }
            else if (where.CompareType == ComparisonOperator.GreaterThan)
            {
                sb.AppendLine($" | where {where.Column} > {parameterName}");
                parameters.Add(new KqlParameter(where.SqlType, parameterName, where.Value));
            }
            else if (where.CompareType == ComparisonOperator.LessThan)
            {
                sb.AppendLine($" | where {where.Column} < {parameterName}");
                parameters.Add(new KqlParameter(where.SqlType, parameterName, where.Value));
            }
            else if (where.CompareType == ComparisonOperator.LessThanOrEquals)
            {
                sb.AppendLine($" | where {where.Column} <= {parameterName}");
                parameters.Add(new KqlParameter(where.SqlType, parameterName, where.Value));
            }
            else if (where.CompareType == ComparisonOperator.In)
            {
                if (where.Value is IEnumerable items)
                {
                    var whereInParameters = new List<string>();
                    var j = 0;
                    foreach (var item in items)
                    {
                        var whereInParameterName = $"{parameterName}_{j}";
                        whereInParameters.Add(whereInParameterName);
                        parameters.Add(new KqlParameter(where.SqlType, whereInParameterName, item));

                        j++;
                    }
                    if (whereInParameters.Count > 0)
                    {
                        var whereInValueString = string.Join(",", whereInParameters);
                        sb.AppendLine($" | where {where.Column} in ({whereInValueString})");
                    }
                    else
                    {
                        // make sure to return no results if an empty where in is used.
                        sb.AppendLine($" | where 1 == 0");
                    }
                }
                else
                {
                    throw new ArgumentException(
                        $"Cannot build where clause. When using {nameof(ComparisonOperator.In)}, supply an IEnumerable as value."
                    );
                }
            }
            else
                throw new NotSupportedException();
        }
    }
}
