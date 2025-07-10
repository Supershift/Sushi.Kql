using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql.UnitTests.QueryBuilder;
public class UnionTest
{
    private readonly QueryBuilder<SalesFact> _qb;
    public UnionTest()
    {
        _qb = new QueryBuilder<SalesFact>(new SalesFactMap());
    }

    [Fact]
    public void SummarizeAndUnionOnTotal()
    {
        string alias = "FilteredSales";
        _qb.Where().Equals(x => x.ProductKey, 2023);
        _qb.As(alias, true);
        _qb.Summarize().Agg(a => a.Count("ItemsSold")).By(x => x.CustomerKey);
        _qb.Union(ub => ub.AddUnion(alias, q => q.Summarize().Agg(a => a.Count("ItemsSold"))));
        

        var kqlQuery = _qb.ToKqlString();

        // Assert
        var expectedMakeSeries = @"declare query_parameters(p0:long);
SalesFact
| where ProductKey == p0
| as hint.materialized=true FilteredSales
| summarize ItemsSold = count() by CustomerKey
| union (FilteredSales
| summarize ItemsSold = count())";
        Assert.Equal(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void SummarizeAndUnionOnTotal_WithSourcename()
    {
        string alias = "FilteredSales";
        string sourceColumnName = "Source";
        _qb.Where().Equals(x => x.ProductKey, 2023);
        _qb.As(alias, true);
        _qb.Summarize().Agg(a => a.Count("ItemsSold")).By(x => x.CustomerKey);
        _qb.Union(ub => {
            ub.WithSourceName(sourceColumnName);
            ub.AddUnion(alias, q => q.Summarize().Agg(a => a.Count("ItemsSold")));
        });

        var kqlQuery = _qb.ToKqlString();

        // Assert
        var expectedMakeSeries = @"declare query_parameters(p0:long);
SalesFact
| where ProductKey == p0
| as hint.materialized=true FilteredSales
| summarize ItemsSold = count() by CustomerKey
| union withsource=Source (FilteredSales
| summarize ItemsSold = count())";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }
}
