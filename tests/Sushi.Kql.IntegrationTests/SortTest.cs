using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;


[Collection(nameof(AdxTestContainerFixture))]
public class SortTest
{
    private readonly QueryClient _queryClient;

    public SortTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());
    }

    [Fact]
    public async Task SortByBuilder()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        qb.Sort().By(x => x.DateKey, SortDirection.ASC, NullsPlacement.NullsFirst);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int count = 0;
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(10000, count);
    }
}
