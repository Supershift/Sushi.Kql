using System.Collections.Concurrent;
using System.Reflection;

namespace Sushi.Kql
{
    /// <summary>
    /// Provides methods to map class types to DataMaps
    /// </summary>
    public class DataMapProvider
    {
        /// <summary>
        /// Gets a collection of key value pairs with default relations between <see cref="DataMap"/> classes and mapped classes.
        /// </summary>
        private ConcurrentDictionary<Type, Type> _dataMapTypes { get; } = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Sets the DataMap<typeparamref name="T"/> to use when resolving queries for <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        public void AddMapping<T, Y>() where Y : DataMap<T>
        {
            _dataMapTypes[typeof(T)] = typeof(Y);
        }

        /// <summary>
        /// Sets the type to use when resolving queries for another type.
        /// </summary>
        /// <param name="classToMap"></param>
        /// <param name="dataMap"></param>
        public void AddMapping(Type classToMap, Type dataMap)
        {
            // check if dataMap is of type DataMap<classToMap>
            if (!ReflectionHelper.IsSubClassOfGeneric(typeof(DataMap<>), dataMap))
                throw new ArgumentException($"{dataMap} is not of type DataMap<{classToMap}>");            

            _dataMapTypes[classToMap] = dataMap;
        }

        /// <summary>
        /// Returns an instance of DataMap<typeparamref name="T"/> for <typeparamref name="T"/> if declared or throws an exception if none found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>        
        public DataMap<T> GetMapForType<T>()
        {
            var type = typeof(T);
            Type? dataMapType = null;
            if (_dataMapTypes.ContainsKey(type))
            {
                dataMapType = _dataMapTypes[type];
            }

            if (dataMapType != null)
            {
                var dataMap = Activator.CreateInstance(dataMapType, true) as DataMap<T>;
                if (dataMap != null)
                    return dataMap;
            }

            throw new Exception($"No DataMap defined for type {typeof(T)}");
        }
    }
}
