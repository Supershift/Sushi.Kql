namespace Sushi.Kql.UnitTests.QueryBuilder;
public class NestedConditionTest
{
    [Fact]
    public void WhereClause_WithNestedConditions_WrapsNestedPartInParentheses()
    {
        var qb = new QueryBuilder<SalesFact>(new SalesFactMap());

        var where = qb.Where();
        where.Equals(x => x.ProductKey, 2023);
        where.Nested(n => n
            .Add(x => x.SalesAmount, 100f, ComparisonOperator.GreaterThan, Kql.ConjunctionOperator.Or)
            .Add(x => x.TotalCost, 100f, ComparisonOperator.LessThan, Kql.ConjunctionOperator.Or)
        );

        var kqlQuery = qb.ToKqlString();

        // Assert
        var expectedWhereClause = @"where ProductKey == p0 and (SalesAmount > p1 or TotalCost < p2)";
        Assert.EndsWith(expectedWhereClause, kqlQuery);
    }
}
