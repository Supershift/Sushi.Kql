using System.Text.Json;
using Kusto.Data.Common;
using Kusto.Ingest;

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
        DataMap<T> mapping
    )
    {
        var ingestionProperties = new KustoQueuedIngestionProperties(database, mapping.TableName)
        {
            Format = DataSourceFormat.json,
            IngestionMapping = new IngestionMapping() { IngestionMappings = mapping.GetIngestionMapping() }
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
        DataMap<T> mapping
    )
    {
        var ingestionProperties = new KustoQueuedIngestionProperties(database, mapping.TableName)
        {
            Format = DataSourceFormat.multijson,
            IngestionMapping = new IngestionMapping() { IngestionMappings = mapping.GetIngestionMapping() },
            FlushImmediately = true,
        };

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, items);
        stream.Position = 0;
        _ = await ingestClient.IngestFromStreamAsync(stream, ingestionProperties);
    }

    
}
