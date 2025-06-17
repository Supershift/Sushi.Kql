using System.Text;

namespace Sushi.Kql.UnitTests;
public class SortBuilderTest
{
    [Fact]
    public void SortBy()
    {
        var map = new SalesFactMap();
        var sb = new StringBuilder();

        var sort = new SortBuilder<SalesFact>(map, sb);

        sort.By(x => x.TotalCost)
            .By(x => x.SalesAmount);

        var result = sb.ToString();
        Assert.Equal("| sort by TotalCost, SalesAmount", result);
    }

    [Fact]
    public void SortBy_NullsPlacement()
    {
        var map = new SalesFactMap();
        var sb = new StringBuilder();

        var sort = new SortBuilder<SalesFact>(map, sb);

        sort.By(x => x.TotalCost, SortDirection.DESC, NullsPlacement.NullsLast)
            .By(x => x.SalesAmount, SortDirection.ASC, NullsPlacement.NullsFirst);

        var result = sb.ToString();
        Assert.Equal("| sort by TotalCost desc nulls last, SalesAmount asc nulls first", result);
    }
}
