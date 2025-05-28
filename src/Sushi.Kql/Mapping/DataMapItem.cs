using System.Reflection;
using Kusto.Data.Common;

namespace Sushi.Kql.Mapping;

public class DataMapItem
{
    public DataMapItem(string[] path, string column, KqlDataType dataType, Type memberType, ColumnMapping ingestionMapping, List<MemberInfo> memberInfoTree)
    {
        Path = path;
        Column = column;
        DataType = dataType;
        MemberType = memberType;
        IngestionMapping = ingestionMapping;
        MemberInfoTree = memberInfoTree;
    }

    public string[] Path { get; internal set; }
    public string Column { get; internal set; }
    public KqlDataType DataType { get; internal set; }
    public Type MemberType { get; internal set; }
    public ColumnMapping IngestionMapping { get; internal set; }
    /// <summary>
    /// Gets or sets <see cref="MemberInfo"/> about the mapped field or property.
    /// </summary>
    public List<MemberInfo> MemberInfoTree { get; } = new List<MemberInfo>();

    /// <summary>
    /// If true, this mapping is read-only and won't be used for ingestion.
    /// </summary>
    public bool IsReadOnly { get; internal set; }
}

