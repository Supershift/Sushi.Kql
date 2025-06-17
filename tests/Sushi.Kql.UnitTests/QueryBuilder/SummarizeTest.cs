using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.TimeSeries;

namespace Sushi.Kql.UnitTests.QueryBuilder;
public class SummarizeTest
{
    private readonly QueryBuilder<SalesFact> _queryBuilder;
    public SummarizeTest()
    {
        _queryBuilder = new QueryBuilder<SalesFact>(new SalesFactMap());
    }

    [Fact]
    public void SummarizeByProductKey()
    {
        _queryBuilder.Summarize().By(x => x.ProductKey);
        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| summarize  by ProductKey";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void SummarizeByProductKeyWithAlias()
    {
        string alias = "Product";

        _queryBuilder.Summarize().By(x => x.ProductKey, alias);
        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| summarize  by Product = ProductKey";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void SummarizeByBuilderProductKeyWithAlias()
    {
        string alias = "Product";

        _queryBuilder.Summarize().Agg(a => a.Count()).By(b => b.Term(x => x.ProductKey, alias));

        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| summarize count() by Product = ProductKey";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }

    [Fact]
    public void SummarizeByBuilderDateBin()
    {
        string alias = "Day";

        _queryBuilder.Summarize().By(b => b.Bin(x => x.DateKey, "1d", alias));


        var kqlQuery = _queryBuilder.ToKqlString();

        // Assert
        var expectedMakeSeries = @"| summarize  by Day = bin(DateKey, p0)";
        Assert.Contains(expectedMakeSeries, kqlQuery);
    }
}
