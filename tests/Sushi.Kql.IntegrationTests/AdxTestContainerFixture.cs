using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Testcontainers.Kusto;

namespace Sushi.Kql.TestData.Fixture;

public class AdxTestContainerFixture : IAsyncLifetime
{
    private readonly KustoContainer _kustoContainer;
    public const string DatabaseName = "ContosoSales";
    private const string CreateTableCommand =
        ".create table SalesFact (SalesAmount: real, TotalCost: real, DateKey: datetime, ProductKey: long, CustomerKey: long)";


    public AdxTestContainerFixture()
    {
        _kustoContainer = new KustoBuilder()
            .WithResourceMapping("SalesFact.csv", "data/SalesFact.csv") // used for seeding default data
            .Build();
    }

    public ICslQueryProvider GetQueryProvider()
    {
        var provider = KustoClientFactory.CreateCslQueryProvider(_kustoContainer.GetConnectionString());
        provider.DefaultDatabaseName = DatabaseName;
        return provider;
    }

    public QueryClient GetQueryClient()
    {
        return new QueryClient(GetQueryProvider());
    }

    public async Task DisposeAsync()
    {
        await _kustoContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await _kustoContainer.StartAsync(cts.Token);

        using var client = KustoClientFactory.CreateCslAdminProvider(_kustoContainer.GetConnectionString());
        await client.ExecuteControlCommandAsync(
            "",
            $".create database {DatabaseName} persist(\"/kustodata/dbs/md\", \"/kustodata/dbs/data\")"
        );

        // create table
        await client.ExecuteControlCommandAsync(DatabaseName, CreateTableCommand);

        // fill table
        await client.ExecuteControlCommandAsync(
            DatabaseName,
            ".ingest into table SalesFact(@\"/data/SalesFact.csv\") with (ignoreFirstRecord=true)"
        );
    }

    [CollectionDefinition(nameof(AdxTestContainerFixture))]
    public class AdxTestContainerCollectionFixture : ICollectionFixture<AdxTestContainerFixture> { }
}
