using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class SummarizeBuilderTest
{   
    private readonly ICslQueryProvider _queryClient;

    public SummarizeBuilderTest(AdxTestContainerFixture fixture)
    {
        _queryClient = fixture.GetQueryClient();
    }

    [Fact]
    public async Task SummarizeOnDCount()
    {
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        qb.Summarize().On(a => a.DCount(x => x.ProductKey, "productcount"));//.By(x => x.DepthInMeters);

        var kqlQuery = qb.Build();
        var parameters = qb.GetParameters();
        var properties = new ClientRequestProperties();
        if(parameters.Count > 0)
            properties.SetParameters(parameters);
        
        var reader = await _queryClient.ExecuteQueryAsync(map.TableName, kqlQuery, properties);
        reader.Read();
        var result = reader.GetInt64(0);
        Assert.Equal(519, result);
    }

    [Fact]
    public async Task SummarizeOnDCount_ByDate()
    {
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);
        qb.Summarize().On(a => a.DCount(x => x.ProductKey, "productcount")).By(x => x.DateKey);

        var kqlQuery = qb.Build();
        var parameters = qb.GetParameters();
        var properties = new ClientRequestProperties();
        if (parameters.Count > 0)
            properties.SetParameters(parameters);

        var reader = await _queryClient.ExecuteQueryAsync(map.TableName, kqlQuery, properties);
        int rowCount = 0;
        while(reader.Read())
        {
            rowCount++;
        }        
        
        Assert.Equal(6, rowCount);
    }


}
