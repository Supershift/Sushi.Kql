using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.AggregationFunctions;
internal static class Sample
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

    internal static void DoIt()
    {
        var qb = new KqlQueryBuilder<Something>(new SomethingMap());
        qb.Summarize(Aggs.DistinctCount<Something>(x => x.Name, "name"), Aggs.DistinctCount<Something>(x => x.Name, "name")).By(x => x.Age);
    }

    internal static void DoIt2()
    {
        var qb = new KqlQueryBuilder<Something>(new SomethingMap());
        qb.Summarize().On(a => a.DistinctCount(x => x.Name, "name")).By(x => x.Age);
    }

    internal static void DoIt2B()
    {
        var qb = new KqlQueryBuilder<Something>(new SomethingMap());
        qb.Summarize().On(a => [a.DistinctCount(x => x.Name, "name"), a.DistinctCount(x=>x.Id, "id")]).By(x => x.Age);
    }
}
