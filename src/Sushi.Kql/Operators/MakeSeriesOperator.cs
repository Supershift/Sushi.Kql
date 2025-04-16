using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql.Operators
{
    /// <summary>
    /// Represent a 'make-series' operator, used to create series of specified aggregated values along a specified axis.
    /// </summary>
    public class MakeSeriesOperator : IKqlOperator
    {
        /// <summary>
        /// Creates a new instance of <see cref="MakeSeriesOperator"/>.
        /// </summary>        
        public MakeSeriesOperator(string onColumn, IAggregationFunction aggregation, object from, object to, string step, bool nonEmptyKind = true)
        {
            OnColumn = onColumn;
            Aggregation = aggregation;
            Start = from;
            End = to;
            Step = step;
            NonEmptyKind = nonEmptyKind;
        }

        /// <summary>
        /// Column to aggregate on.
        /// </summary>
        public string OnColumn { get; }
        public IAggregationFunction Aggregation { get; }

        /// <summary>
        /// Inclusive lower bound of the y-axis.
        /// </summary>
        public object Start { get; }
        /// <summary>
        /// Exclusive upper bound of the y-axis.
        /// </summary>
        public object End { get; }
        /// <summary>
        /// Bin / bucket size for the y-axis.
        /// </summary>
        public string Step { get; }

        /// <summary>
        /// If true, the series will include empty buckets for missing values.
        /// </summary>
        public bool NonEmptyKind { get; }

        public string ToKql(ParameterCollection parameters)
        {
            var start = parameters.Add(KqlDataType.DateTime, Start);
            var end = parameters.Add(KqlDataType.DateTime, End);
            return $"make-series {(NonEmptyKind ? "kind=nonempty" : "")} {Aggregation.ToKql(parameters)} default = 0 on {OnColumn} from {start} to {end} step {Step}";
        }
    }
}
