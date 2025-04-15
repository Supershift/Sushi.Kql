using Sushi.Kql.Operators;
using System.Linq.Expressions;
using System.Text;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds Summarize KQL statements, used for grouping and aggregating data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SummarizeBuilder<T>
    {
        private readonly DataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;
        private bool _isFirst;

        internal SummarizeBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
        {
            _map = map;
            _builder = builder;
            _parameters = parameters;
            _isFirst = true;
            builder.Append("| summarize ");
        }

        public SummarizeBuilder<T> Count(string? alias = null) 
        {
            return Add(new CountFunction(alias));
        }

        public SummarizeBuilder<T> DistinctCount(Expression<Func<T, object?>> expression, string? alias = null, int? accuracy = null)
        {
            var dataProperty = _map.GetDataProperty(expression);
            return Add(new DistinctCountFunction(dataProperty.Column, alias, accuracy));
        }

        public SummarizeBuilder<T> ArgMax(Expression<Func<T, object?>> expressionToMaximize, params Expression<Func<T, object?>>[] expressionsToReturn)
        {
            var maximize = _map.GetDataProperty(expressionToMaximize);
            var returnColumns = expressionsToReturn.Select(x => _map.GetDataProperty(x).Column).ToArray();
            return Add(new ArgMaxFunction(maximize.Column, returnColumns));
        }

        public SummarizeBuilder<T> Add(IAggregationFunction aggregation)
        {
            return Add(aggregation.ToKql(_parameters));
        }

        public SummarizeBuilder<T> Add(string customKql)
        {
            if (_isFirst)
                _isFirst = false;
            else
                _builder.Append(", ");

            _builder.Append(customKql);            
            return this;
        }

        public SummarizeBuilder<T> By(Expression<Func<T, object?>> expression)
        {
            var dataProperty = _map.GetDataProperty(expression);
            _builder.Append($" by {dataProperty.Column}");
            return this;
        }
    }
}
