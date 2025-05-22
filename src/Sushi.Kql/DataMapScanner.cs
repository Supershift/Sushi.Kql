using System.Reflection;

namespace Sushi.Kql
{
    /// <summary>
    /// Provides methods to scan assemblies for <see cref="DataMap"/> implementations.
    /// </summary>
    public static class DataMapScanner
    {
        /// <summary>
        /// Scans the specified assemblies for <see cref="DataMap"/> implementations and adds them to the datamap provider.
        /// Note: DataMaps containing generic parameters are ignored.
        /// </summary>        
        public static void Scan(DataMapProvider dataMapProvider, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                // get all types that are subclasses of DataMap<>
                var dataMapTypes = assembly.GetTypes().Where(x => ReflectionHelper.IsSubClassOfGeneric(typeof(DataMap<>), x) && !x.IsAbstract && !x.ContainsGenericParameters);                

                // for each type, determine what T is in DataMap<T>, and add it to the provider
                foreach (var dataMapType in dataMapTypes)
                {
                    var candidate = dataMapType;
                    do
                    {
                        if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(DataMap<>))
                        {
                            dataMapProvider.AddMapping(candidate.GenericTypeArguments[0], dataMapType);
                            break;
                        }
                        candidate = candidate.BaseType;
                    }
                    while (candidate?.BaseType != null);
                }
            }
        }

    }
}
