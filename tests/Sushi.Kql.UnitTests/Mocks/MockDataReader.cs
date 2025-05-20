using System.Data;

namespace Sushi.Kql.UnitTests.Mocks;


public class MockDataReader(List<Dictionary<string, object?>> rows) : IDataReader
{
    private readonly List<Dictionary<string, object?>> _rows = rows;
    private int _currentIndex = -1;

    public bool Read()
    {
        _currentIndex++;
        return _currentIndex < _rows.Count;
    }

    public object this[string name] => _rows[_currentIndex][name] ?? DBNull.Value;

    public int FieldCount => _rows[_currentIndex].Count;

    public string GetName(int i) => _rows[_currentIndex].Keys.ElementAt(i);

    public object GetValue(int i) => _rows[_currentIndex].Values.ElementAt(i) ?? DBNull.Value;

    public int GetOrdinal(string name) => _rows[_currentIndex].Keys.ToList().IndexOf(name);

    public bool IsDBNull(int i) => GetValue(i) == DBNull.Value;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) { }

    public bool NextResult() => false;

    public int Depth => 0;
    public bool IsClosed => false;
    public int RecordsAffected => 0;

    public void Close() { }

    public bool GetBoolean(int i) => (bool)GetValue(i);

    public byte GetByte(int i) => (byte)GetValue(i);

    public char GetChar(int i) => (char)GetValue(i);

    public IDataReader GetData(int i) => throw new NotSupportedException();

    public string GetDataTypeName(int i) => GetFieldType(i).Name;

    public DateTime GetDateTime(int i) => (DateTime)GetValue(i);

    public decimal GetDecimal(int i) => (decimal)GetValue(i);

    public double GetDouble(int i) => (double)GetValue(i);

    public Type GetFieldType(int i) => GetValue(i).GetType();

    public float GetFloat(int i) => (float)GetValue(i);

    public Guid GetGuid(int i) => (Guid)GetValue(i);

    public short GetInt16(int i) => (short)GetValue(i);

    public int GetInt32(int i) => (int)GetValue(i);

    public long GetInt64(int i) => (long)GetValue(i);

    public string GetString(int i) => (string)GetValue(i);

    public int GetValues(object[] values)
    {
        var row = _rows[_currentIndex].Values.ToArray();
        Array.Copy(row, values, row.Length);
        return row.Length;
    }

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) =>
        throw new NotSupportedException();

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) =>
        throw new NotSupportedException();

    public DataTable? GetSchemaTable() => throw new NotImplementedException();

    public object this[int i] => GetValue(i);
}
