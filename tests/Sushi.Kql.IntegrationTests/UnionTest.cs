using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class UnionTest
{
    private readonly QueryClient _queryClient;

    public UnionTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());        
    }

    [Fact]
    public async Task SummarizeAndUnionOnTotal()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        string alias = "FilteredSales";
        qb.Where().Equals(x => x.ProductKey, 2023);
        qb.As(alias, true);
        qb.Summarize().Agg(a=>a.Count("ItemsSold")).By(x => x.CustomerKey);
        qb.Union(alias, u =>
        {
            u.Summarize().Agg(a => a.Count("ItemsSold"));
        });

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int rowCount = 0;
        while (reader.Read())
        {
            rowCount++;
        }

        // assert
        Assert.Equal(2, reader.FieldCount);        
        Assert.Equal(4, rowCount);
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
