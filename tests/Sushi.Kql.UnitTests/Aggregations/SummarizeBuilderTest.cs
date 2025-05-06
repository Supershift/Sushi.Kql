using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.AggregationFunctions;

namespace Sushi.Kql.UnitTests.Aggregations;
internal class SummarizeBuilderTest
{
    private record Something
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required int Age { get; init; }
    }

    private class SomethingMap : DataMap<Something>
    {
        public override string TableName => "somethings";
        public SomethingMap()
        {
            Map(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Age);
        }
    }

    internal static void SummarizeOnSingle()
    {
        var qb = new KqlQueryBuilder<Something>(new SomethingMap());
        qb.Summarize().On(a => a.DCount(x => x.Name, "name")).By(x => x.Age);
    }

    internal static void SummarizeOnMultiple()
    {
        var qb = new KqlQueryBuilder<Something>(new SomethingMap());
        qb.Summarize().On(a => [a.DCount(x => x.Name, "name"), a.DCount(x => x.Id, "id")]).By(x => x.Age);
    }
}
