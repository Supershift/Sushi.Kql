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
        qb.Summarize().Agg(a => a.Count("ItemsSold")).By(x => x.CustomerKey);
        qb.Union(u =>
        {
            u.Summarize().Agg(a => a.Count("ItemsSold"));
        },
        queryBuilderTableName: alias);

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
    public async Task SummarizeAndUnionOnTotal_WithSourcename()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        string alias = "FilteredSales";
        string sourceColumnName = "Source";
        qb.Where().Equals(x => x.ProductKey, 2023);
        qb.As(alias, true);
        qb.Summarize().Agg(a => a.Count("ItemsSold")).By(x => x.CustomerKey);
        qb.Union(u =>
        {
            u.Summarize().Agg(a => a.Count("ItemsSold"));
        },
        queryBuilderTableName: alias,
        withsourceName: sourceColumnName);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int rowCount = 0;
        while (reader.Read())
        {
            rowCount++;
        }

        // assert
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal(sourceColumnName, reader.GetName(0));
        Assert.Equal(4, rowCount);
    }
}
