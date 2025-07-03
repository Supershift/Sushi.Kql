using Kusto.Data.Common;
using Sushi.Kql.TestData;
using Sushi.Kql.TestData.Fixture;

namespace Sushi.Kql.IntegrationTests;
[Collection(nameof(AdxTestContainerFixture))]
public class UnionTest
{
    private readonly QueryClient _queryClient;

    public UnionTest(AdxTestContainerFixture fixture)
    {
        _queryClient = new QueryClient(fixture.GetQueryProvider());
    }

    [Fact]
    public async Task SummarizeAndUnionOnTotal()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        string aliasBase = "FilteredSales";                
        qb.Where().Equals(x => x.ProductKey, 2023);
        qb.As(aliasBase, true);
        qb.Summarize().Agg(a => [a.Count("ItemsSold"), a.DCount(x => x.ProductKey, "ProductsSold")]).By(x => x.CustomerKey);
        qb.Union(ub =>
        {   
            ub.AddUnion(query1 => query1.Summarize().Agg(a => a.Count("ItemsSold")), aliasBase);
            ub.AddUnion(query2 => query2.Summarize().Agg(a => a.DCount(x => x.ProductKey, "ProductsSold")), aliasBase);
        });

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int rowCount = 0;
        while (reader.Read())
        {
            rowCount++;            
        }

        // assert
        Assert.Equal(3, reader.FieldCount);
        Assert.Equal(5, rowCount);
    }

    [Fact]
    public async Task SummarizeAndUnionOnTotal_WithSourceName()
    {
        // arrange
        var map = new SalesFactMap();
        var qb = new QueryBuilder<SalesFact>(map);

        const string aliasBase = "FilteredSales";
        const string summed = "Summed";
        const string union1 = "Union1";
        const string union2 = "Union2";
        const string sourceColumnName = "SourceColumn";
        qb.Where().Equals(x => x.ProductKey, 2023);
        qb.As(aliasBase, true);
        qb.Summarize().Agg(a => [a.Count("ItemsSold"), a.DCount(x => x.ProductKey, "ProductsSold")]).By(x => x.CustomerKey);
        qb.As(summed);
        qb.Union(ub =>
        {
            ub.AddUnion(query1 => { query1.Summarize().Agg(a => a.Count("ItemsSold")); query1.As(union1); }, aliasBase);
            ub.AddUnion(query2 => { query2.Summarize().Agg(a => a.DCount(x => x.ProductKey, "ProductsSold")); query2.As(union2); }, aliasBase);
        }, sourceColumnName);

        // act
        var reader = await _queryClient.ExecuteQueryAsync(qb, "ContosoSales");
        int rowCount = 0;
        bool hasSummed = false;
        bool hasUnion1Alias = false;
        bool hasUnion2Alias = false;
        int sourceColumnIndex = reader.GetOrdinal(sourceColumnName);
        while (reader.Read())
        {
            rowCount++;
            switch (reader.GetString(sourceColumnIndex))
            {
                case summed: hasSummed = true; break;
                case union1: hasUnion1Alias = true; break;
                case union2: hasUnion2Alias = true; break;
            }
        }

        // assert
        Assert.Equal(4, reader.FieldCount);
        Assert.Equal(5, rowCount);
        Assert.True(hasSummed);
        Assert.True(hasUnion1Alias);
        Assert.True(hasUnion2Alias);
    }
}
