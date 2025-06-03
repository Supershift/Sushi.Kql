using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class SummarizeTest
{
    private readonly QueryClient _queryClient;

    public SummarizeTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());        
    }

    [Fact]
    public async Task SummarizeByProductKey()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        qb.Summarize().By(x => x.ProductKey);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int count = 0;
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(519, count);
    }

    [Fact]
    public async Task SummarizeByProductKeyWithAlias()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        string alias = "Product";
        
        qb.Summarize().By(x => x.ProductKey, alias);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int count = 0;
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(519, count);
        Assert.Equal(alias, reader.GetName(0));
    }

    [Fact]
    public async Task SummarizeByBuilderProductKeyWithAlias()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        string alias = "Product";

        qb.Summarize().Agg(a=>a.Count()).By(b => b.Term(x => x.ProductKey, alias));

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int count = 0;        
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(519, count);
        Assert.Equal(alias, reader.GetName(0));
    }

    [Fact]
    public async Task SummarizeByBuilderDateBin()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        string alias = "Day";

        qb.Summarize().By(b => b.Bin(x => x.DateKey, "1d", alias));

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");

        int count = 0;
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(5, count);
        Assert.Equal(alias, reader.GetName(0));
    }
}
