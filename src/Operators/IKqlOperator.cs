namespace Sushi.Kql.Operators
{
    /// <summary>
    /// Defines an interface for a KQL Operator.
    /// </summary>
    public interface IKqlOperator
    {
        /// <summary>
        /// Generate KQL for the operator
        /// </summary>        
        string ToKql(ParameterCollection parameters);
    }
}
