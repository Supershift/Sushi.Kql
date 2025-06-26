using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class AggregationTest
{
    private readonly QueryClient _queryClient;

    public AggregationTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());
    }

    [Fact]
    public async Task TakeAny()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        qb.Summarize().Agg(x => x.TakeAny(x => x.TotalCost)).By(x => x.ProductKey);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");

        // assert
        Assert.Equal(2, reader.FieldCount);
    }

    [Fact]
    public async Task TakeAny_WithAlias()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        string alias = "CST";
        qb.Summarize().Agg(x => x.TakeAny(x => x.TotalCost, alias)).By(x => x.ProductKey);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");

        // assert
        Assert.Equal(2, reader.FieldCount);
        Assert.Equal(alias, reader.GetName(1));
    }

    [Fact]
    public async Task TakeAny_MultipleWithAlias()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        string alias1 = "CST";
        string alias2 = "KEY";
        qb.Summarize().Agg(x => x.TakeAny([(x => x.TotalCost, alias1), (x => x.CustomerKey, alias2)])).By(x => x.ProductKey);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");

        // assert
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal(alias1, reader.GetName(1));
        Assert.Equal(alias2, reader.GetName(2));
    }
}
