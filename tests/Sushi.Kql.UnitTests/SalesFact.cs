namespace Sushi.Kql.UnitTests;
public record SalesFact
{
    public required double SalesAmount { get; set; }
    public required double TotalCost { get; set; }
    public required DateTime DateKey { get; set; }
    public required long ProductKey { get; set; }
    public required long CustomerKey { get; set; }
}
