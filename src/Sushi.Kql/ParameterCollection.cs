using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql
{
    public class ParameterCollection
    {
        private readonly Dictionary<string, KqlParameter> _parameters = [];

        public string Add(SqlDbType sqlDbType, object? value)
        {
            string parameterName = $"p{_parameters.Keys.Count}";
            var parameter = new KqlParameter(sqlDbType, parameterName, value);
            _parameters.Add(parameter.Name, parameter);

            return parameterName;
        }

        public IEnumerable<KqlParameter> GetParameters()
        {
            return _parameters.Values;
        }
    }
}
