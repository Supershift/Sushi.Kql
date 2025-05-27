using System.Linq.Expressions;
using System.Text;

namespace Sushi.Kql
{
    /// <summary>
    /// Builds the select part of a KQL query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectBuilder<T>
    {
        private readonly DataMap<T> _map;
        private readonly StringBuilder _builder;
        private readonly ParameterCollection _parameters;
        private bool _isFirst;

        internal SelectBuilder(DataMap<T> map, StringBuilder builder, ParameterCollection parameters)
        {
            _map = map;
            _builder = builder;
            _parameters = parameters;
            _isFirst = true;            
        }

        /// <summary>
        /// Creates series of specified aggregated values along a specified axis.
        /// </summary>        
        public MakeSeriesBuilder<T> MakeSeries(MakeSeriesKind kind = MakeSeriesKind.Empty)
        {
            var builder = new MakeSeriesBuilder<T>(kind, _map, _builder, _parameters);
            return builder;
        }

        /// <summary>
        /// Adds a custom KQL statement to the query.
        /// </summary>
        /// <param name="customKql"></param>
        /// <returns></returns>
        public SelectBuilder<T> Add(string customKql)
        {
            if (_isFirst)
                _isFirst = false;
            else
                _builder.Append(", ");

            _builder.Append(customKql);
            return this;
        }
    }
}
