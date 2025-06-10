using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class MvExpandTest
{
    private readonly QueryClient _queryClient;

    public MvExpandTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());
    }

    [Fact]
    public async Task ExpandTimeSeries()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        int days = 14;
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(days);

        string countAlias = "SalesCount";

        qb.Select().MakeSeries().Agg(a => a.Count(countAlias)).On(x => x.DateKey).From(from).To(to).Step("1d");
        qb.MvExpand().Add(countAlias, KqlDataType.Long).Add(x => x.DateKey);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int count = 0;
        while (reader.Read())
        {
            count++;
        }

        // assert
        Assert.Equal(days, count);
        Assert.Equal(2, reader.FieldCount);
    }

}
