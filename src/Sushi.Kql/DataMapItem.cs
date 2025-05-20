using Kusto.Data.Common;

namespace Sushi.Kql;

public class DataMapItem
{
    public DataMapItem(string[] path, string column, KqlDataType dataType, Type memberType, ColumnMapping ingestionMapping)
    {
        Path = path;
        Column = column;
        DataType = dataType;
        MemberType = memberType;
        IngestionMapping = ingestionMapping;
    }

    public string[] Path { get; internal set; }
    public string Column { get; internal set; }
    public KqlDataType DataType { get; internal set; }
    public Type MemberType { get; internal set; }
    public ColumnMapping IngestionMapping { get; internal set; }

    /// <summary>
    /// If true, this mapping is read-only and won't be used for ingestion.
    /// </summary>
    public bool IsReadOnly { get; internal set; }
}

