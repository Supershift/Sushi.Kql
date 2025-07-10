using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Data.Common;
using Kusto.Data.Exceptions;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class RestrictedBuilderTest
{
    private readonly QueryClient _queryClient;
    private readonly SalesFactMap _map;    

    public RestrictedBuilderTest(AdxTestContainerFixture fixture)
    {
        _queryClient = fixture.GetQueryClient();
        _map = new SalesFactMap();
        
    }

    [Fact]
    public async Task GetRestrictedData()
    {
        string viewName = "RestrictedData";
        var query = new QueryBuilder<SalesFact>(_map, viewName);
        query.RestrictAccessTo(rb =>
        {
            rb.AddView(viewName, vb => vb.Where().Equals(x => x.CustomerKey, 6791));
        });

        var result = await _queryClient.GetAllAsync(query, "ContosoSales");

        Assert.Equal(4, result.Data.Count);
    }

    [Fact]
    public async Task GetRestrictedData_NotAllowedResource()
    {
        string viewName = "RestrictedData";
        var query = new QueryBuilder<SalesFact>(_map, "SalesFact");
        query.RestrictAccessTo(rb =>
        {
            rb.AddView(viewName, vb => vb.Where().Equals(x => x.CustomerKey, 6791));
        });

        var act = async () => _ = await _queryClient.GetAllAsync(query, "ContosoSales");

        var ex = await Assert.ThrowsAsync<Sushi.Kql.Exceptions.QueryExecutionException>(act);
        _ = Assert.IsType<SemanticException>(ex.InnerException);        
    }
}
