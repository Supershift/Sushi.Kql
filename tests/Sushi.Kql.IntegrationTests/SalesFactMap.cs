using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.IntegrationTests;
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
