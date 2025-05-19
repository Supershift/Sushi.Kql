using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.TestData;
public record SalesFact
{
    public required double SalesAmount { get; set; }
    public required double TotalCost { get; set; }
    public required DateTime DateKey { get; set; }
    public required long ProductKey { get; set; }
    public required long CustomerKey { get; set; }
}
