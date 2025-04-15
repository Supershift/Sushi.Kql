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
        private readonly Dictionary<string, Parameter> _parameters = [];

        public string Add(KqlDataType kqlDataType, object? value)
        {
            string parameterName = $"p{_parameters.Keys.Count}";
            var parameter = new Parameter(kqlDataType, parameterName, value);
            _parameters.Add(parameter.Name, parameter);

            return parameterName;
        }

        public IEnumerable<Parameter> GetParameters()
        {
            return _parameters.Values;
        }
    }
}
