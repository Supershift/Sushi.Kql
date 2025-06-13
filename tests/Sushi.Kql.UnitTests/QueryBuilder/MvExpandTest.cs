using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql.UnitTests.QueryBuilder;
public class MvExpandTest
{
    private readonly QueryBuilder<SalesFact> _queryBuilder;
    public MvExpandTest()
    {
        _queryBuilder = new QueryBuilder<SalesFact>(new SalesFactMap());
    }

    [Fact]
    public void ExpandTimeSeries()
    {
        int days = 14;
        var from = new DateTime(2007, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(days);

        string countAlias = "SalesCount";

        _queryBuilder.Select().MakeSeries().Agg(a => a.Count(countAlias)).On(x => x.DateKey).From(from).To(to).Step("1d");
        _queryBuilder.MvExpand().Add(countAlias, KqlDataType.Long).Add(x => x.DateKey);
        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| make-series SalesCount = count() on DateKey from p0 to p1 step 1d| mv-expand SalesCount to typeof(long),DateKey to typeof(datetime)";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }
}
