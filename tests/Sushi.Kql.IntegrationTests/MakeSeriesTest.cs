using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Data.Common;
using Sushi.Kql.TestData.Fixture;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class MakeSeriesTest
{
    private readonly ICslQueryProvider _queryClient;
    private readonly QueryBuilder<SalesFact> _queryBuilder;

    public MakeSeriesTest(AdxTestContainerFixture fixture)
    {
        _queryClient = fixture.GetQueryProvider();
        var map = new SalesFactMap();
        _queryBuilder = new QueryBuilder<SalesFact>(map);
    }

    [Fact]
    public async Task MakeSeries_NonEmpty()
    {
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(14);

        _queryBuilder.Select().MakeSeries(MakeSeriesKind.NonEmpty).Agg(a => a.Count("Sales")).On(x => x.DateKey).From(from).To(to).Step("1d");

        var kqlQuery = _queryBuilder.ToKqlString();
        var parameters = _queryBuilder.GetParameters();
        var properties = new ClientRequestProperties();
        if (parameters.Count > 0)
            properties.SetParameters(parameters);

        var reader = await _queryClient.ExecuteQueryAsync("ContosoSales", kqlQuery, properties);
        int count = 0;
        while (reader.Read())
        {
            Assert.Equal(2, reader.FieldCount);
            count++;
        }
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task MakeSeries_By()
    {
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(14);

        _queryBuilder.Select().MakeSeries().Agg(a => a.Count("Sales")).On(x => x.DateKey).From(from).To(to).Step("1d").By(x=>x.ProductKey);

        var kqlQuery = _queryBuilder.ToKqlString();
        var parameters = _queryBuilder.GetParameters();
        var properties = new ClientRequestProperties();
        if (parameters.Count > 0)
            properties.SetParameters(parameters);

        var reader = await _queryClient.ExecuteQueryAsync("ContosoSales", kqlQuery, properties);
        int count = 0;
        while (reader.Read())
        {
            Assert.Equal(3, reader.FieldCount);
            count++;
        }
        Assert.Equal(519, count);
    }
}
