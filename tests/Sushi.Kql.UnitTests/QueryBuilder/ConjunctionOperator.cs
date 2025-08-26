namespace Sushi.Kql.UnitTests.QueryBuilder;

public class ConjunctionOperator
{
    [Fact]
    public void WhereClause_WithMultipleAndConditions_DefaultJoinsWithAnd()
    {
        var qb = new QueryBuilder<SalesFact>(new SalesFactMap());

        var where = qb.Where();
        where.Add(x => x.ProductKey, 2023, ComparisonOperator.GreaterThan);
        where.Add(x => x.SalesAmount, 100f, ComparisonOperator.GreaterThan);
        where.Add(x => x.TotalCost, 100f, ComparisonOperator.GreaterThan);

        var kqlQuery = qb.ToKqlString();

        // Assert
        var expected = @"where ProductKey == p0 and SalesAmount > p1 and TotalCost > p2";
        Assert.EndsWith(expected, kqlQuery);
    }

    [Fact]
    public void WhereClause_WithOrCondition_UsesOr()
    {
        var qb = new QueryBuilder<SalesFact>(new SalesFactMap());

        var where = qb.Where();
        where.Add(x => x.ProductKey, 2023, ComparisonOperator.GreaterThan);
        where.Add(x => x.SalesAmount, 100f, ComparisonOperator.GreaterThan, Kql.ConjunctionOperator.Or);

        var kqlQuery = qb.ToKqlString();

        // Assert
        var expected = @"where ProductKey == p0 or SalesAmount > p1";
        Assert.EndsWith(expected, kqlQuery);
    }

}
