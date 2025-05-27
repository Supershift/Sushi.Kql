using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class SummarizeTest
{
    private readonly ICslQueryProvider _queryClient;

    public SummarizeTest(AdxTestContainerFixture fixture)
    {
        _queryClient = fixture.GetQueryProvider();
    }

    [Fact]
    public async Task SummarizeByProductKey()
    {
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        qb.Summarize().By(x => x.ProductKey);

        var kqlQuery = qb.ToKqlString();
        var parameters = qb.GetParameters();
        var properties = new ClientRequestProperties();
        if (parameters.Count > 0)
            properties.SetParameters(parameters);

        var reader = await _queryClient.ExecuteQueryAsync("ContosoSales", kqlQuery, properties);
        int count = 0;
        while (reader.Read())
        {
            count++;
        }
        Assert.Equal(520, count);
    }
}
