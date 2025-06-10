using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sushi.Kql.Mapping;

namespace Sushi.Kql
{
    public class ParameterCollection
    {
        private readonly Dictionary<string, Parameter> _parameters = [];

        public int Count => _parameters.Count;

        public string Add<T>(T? value)
        {
            var kqlType = Conversion.GetKqlDataType(typeof(T));
            return Add(kqlType, value);
        }

        public string Add(KqlDataType kqlDataType, object? value)
        {
            string parameterName = $"p{_parameters.Keys.Count}";
            var parameter = new Parameter(kqlDataType, parameterName, value);
            _parameters.Add(parameter.Name, parameter);

            return parameterName;
        }

        public IReadOnlyCollection<Parameter> GetParameters()
        {
            return _parameters.Values;
        }
    }
}
