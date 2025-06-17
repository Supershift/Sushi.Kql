using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql.UnitTests.QueryBuilder;
public class MakeSeriesTest
{
    private readonly QueryBuilder<SalesFact> _queryBuilder;
    public MakeSeriesTest()
    {
        _queryBuilder = new QueryBuilder<SalesFact>(new SalesFactMap());
    }

    [Fact]
    public void MakeSeries_NonEmpty()
    {
        int days = 14;
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(days);

        _queryBuilder.Select().MakeSeries(MakeSeriesKind.NonEmpty).Agg(a => a.Count("Sales")).On(x => x.DateKey).From(from).To(to).Step("1d");

        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| make-series  kind=nonempty Sales = count() on DateKey from p0 to p1 step 1d";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void MakeSeries_MultipleSeries()
    {
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(14);

        _queryBuilder.Select().MakeSeries().Agg(a => [a.Count("Sales"), a.DCount(x => x.ProductKey, "ProductCount")]).On(x => x.DateKey).From(from).To(to).Step("1d");

        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| make-series Sales = count(), ProductCount = dcount(ProductKey) on DateKey from p0 to p1 step 1d";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void MakeSeries_By()
    {
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(14);

        _queryBuilder.Select().MakeSeries().Agg(a => a.Count("Sales")).On(x => x.DateKey).From(from).To(to).Step("1d").By(x => x.ProductKey);

        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| make-series Sales = count() on DateKey from p0 to p1 step 1d by ProductKey";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

}
