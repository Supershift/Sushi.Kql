namespace Sushi.Kql.AggregationFunctions
{
    public interface IAggregationFunction
    {
        /// <summary>
        /// Generate KQL for the function
        /// </summary>        
        string ToKql(ParameterCollection parameters);
    }
}
