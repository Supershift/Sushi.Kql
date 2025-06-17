using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Cloud.Platform.Data;
using Kusto.Data.Common;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class QueryClientTest
{
    private readonly ICslQueryProvider _cslQueryProvider;
    private readonly SalesFactMap _map;

    public QueryClientTest(AdxTestContainerFixture fixture)
    {
        _cslQueryProvider = fixture.GetQueryProvider();
        _map = new SalesFactMap();

    }

    [Fact]
    public async Task ExecuteQueryTest()
    {
        // arrange
        var queryBuilder = new QueryBuilder<SalesFact>(_map);

        var client = new QueryClient(_cslQueryProvider);

        // act
        using var reader = await client.ExecuteQueryAsync(queryBuilder, "ContosoSales", CancellationToken.None);
        int count = 0;

        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(10000, count);
    }

    [Fact]
    public async Task GetAllTest()
    {
        // arrange
        var queryBuilder = new QueryBuilder<SalesFact>(_map);
        queryBuilder.Take(10);
        var client = new QueryClient(_cslQueryProvider);

        // act
        var result = await client.GetAllAsync(queryBuilder, "ContosoSales", CancellationToken.None);

        // assert
        Assert.Equal(10, result.Data.Count);
        Assert.All(result.Data, item => Assert.NotEqual(0, item.SalesAmount));
    }
}
