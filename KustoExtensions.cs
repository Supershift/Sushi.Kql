using System.Text.Json;
using Kusto.Data.Common;
using Kusto.Ingest;
using Sushi.MicroORM;

namespace Sushi.Kql;

public static class KustoExtensions
{
    /// <summary>
    /// Inserts <typeparamref name="T"/> in the ADX database.
    /// </summary>
    public static async Task IngestAsync<T>(
        this IKustoIngestClient ingestClient,
        string database,
        T item,
        KustoDataMap<T> mapping
    )
    {
        var ingestionProperties = new KustoQueuedIngestionProperties(database, mapping.TableName)
        {
            Format = DataSourceFormat.json,
            IngestionMapping = mapping.IngestionMapping
        };

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, item);
        stream.Position = 0;
        await ingestClient.IngestFromStreamAsync(stream, ingestionProperties);
    }

    /// <summary>
    /// Ingests a collection of entities of <typeparamref name="T"/> using direct ingestion instead of queued ingestion.
    /// </summary>
    public static async Task BulkIngestAsync<T>(
        this IKustoIngestClient ingestClient,
        string database,
        IEnumerable<T> items,
        KustoDataMap<T> mapping
    )
    {
        var ingestionProperties = new KustoQueuedIngestionProperties(database, mapping.TableName)
        {
            Format = DataSourceFormat.multijson,
            IngestionMapping = mapping.IngestionMapping,
            FlushImmediately = true,
        };

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, items);
        stream.Position = 0;
        _ = await ingestClient.IngestFromStreamAsync(stream, ingestionProperties);
    }

    /// <summary>
    /// Gets all records from the ADX database the match the <paramref name="query"/>.
    /// </summary>
    public static async Task<QueryListResult<T>> GetAllAsync<T>(
        this ICslQueryProvider queryProvider,
        string database,
        KustoQuery<T> query,
        KustoDataMap<T> mapping,
        CancellationToken cancellationToken
    )
    {
        var kqlQuery = new KqlStatementGenerator().GenerateKqlQuery(query);
        var properties = new ClientRequestProperties();
        properties.SetParameters(kqlQuery.ParameterDictionary);
        using var reader = await queryProvider.ExecuteQueryAsync(database, kqlQuery.Kql, properties, cancellationToken);
        var result = new QueryListResult<T>();
        while (reader.Read())
        {
            var item = mapping.MapFromReader(reader);
            result.Add(item);
        }

        if (query.Paging != null && reader.NextResult() && reader.Read())
        {
            // add total and page count
            var count = reader.GetInt64(0);
            result.TotalNumberOfRows = Convert.ToInt32(count);
            result.TotalNumberOfPages = (int)Math.Ceiling((double)count / query.Paging!.NumberOfRows);
        }
        return result;
    }
}
