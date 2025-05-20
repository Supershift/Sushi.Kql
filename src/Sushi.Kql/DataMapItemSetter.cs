using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Kql;
/// <summary>
/// Provides methods to build a data map item.
/// </summary>
public class DataMapItemSetter
{
    private readonly DataMapItem _item;

    internal DataMapItemSetter(DataMapItem item)
    {
        _item = item;
    }

    /// <summary>
    /// Sets the <see cref="KqlDataType"/> of the mapped column, overriding the automatically determined type.
    /// </summary>
    /// <param name="kqlDataType"></param>
    /// <returns></returns>
    public DataMapItemSetter KqlDataType(KqlDataType kqlDataType)
    {
        _item.DataType = kqlDataType;
        return this;
    }

    /// <summary>
    /// Sets the mapping to be read-only. This means that the mapping will not be used for ingestion, only for reading.
    /// </summary>
    /// <returns></returns>
    public DataMapItemSetter ReadOnly()
    {
        _item.IsReadOnly = true;
        return this;
    }
}
