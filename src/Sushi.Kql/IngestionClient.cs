using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kusto.Data.Common;
using Kusto.Ingest;

namespace Sushi.Kql;
public class IngestionClient
{
    private readonly IKustoIngestClient _kustoIngestClient;

    public IngestionClient(IKustoIngestClient kustoIngestClient)
    {
        _kustoIngestClient = kustoIngestClient;
    }

    /// <summary>
    /// Inserts <typeparamref name="T"/> in the ADX database.
    /// </summary>
    public async Task IngestAsync<T>(        
        string database,
        T item,
        DataMap<T> mapping
    )
    {
        var ingestionProperties = new KustoQueuedIngestionProperties(database, mapping.TableName)
        {
            Format = DataSourceFormat.json,
            IngestionMapping = mapping.GetIngestionMapping()
        };

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, item);
        stream.Position = 0;
        await _kustoIngestClient.IngestFromStreamAsync(stream, ingestionProperties);
    }
}
