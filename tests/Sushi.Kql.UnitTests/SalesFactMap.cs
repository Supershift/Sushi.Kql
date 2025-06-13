using Sushi.Kql.Mapping;

namespace Sushi.Kql.UnitTests;
public class SalesFactMap : DataMap<SalesFact>
{
    public SalesFactMap()
    {
        Table("SalesFact");
        Map(x => x.CustomerKey);
        Map(x => x.DateKey);
        Map(x => x.ProductKey);
        Map(x => x.SalesAmount);
        Map(x => x.TotalCost);
    }
}
