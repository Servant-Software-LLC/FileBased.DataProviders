namespace System.Data.FileClient;

/// <summary>
/// A wrapper around a DbDataReader that closes the associated connection when the reader is closed/disposed.
/// Used to implement <see cref="CommandBehavior.CloseConnection"/>.
/// </summary>
internal class CloseConnectionDataReader : DbDataReader
{
    private readonly DbDataReader _innerReader;
    private readonly DbConnection _connection;

    public CloseConnectionDataReader(DbDataReader innerReader, DbConnection connection)
    {
        _innerReader = innerReader ?? throw new ArgumentNullException(nameof(innerReader));
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public override int Depth => _innerReader.Depth;
    public override int FieldCount => _innerReader.FieldCount;
    public override bool HasRows => _innerReader.HasRows;
    public override bool IsClosed => _innerReader.IsClosed;
    public override int RecordsAffected => _innerReader.RecordsAffected;

    public override object this[int ordinal] => _innerReader[ordinal];
    public override object this[string name] => _innerReader[name];

    public override bool GetBoolean(int ordinal) => _innerReader.GetBoolean(ordinal);
    public override byte GetByte(int ordinal) => _innerReader.GetByte(ordinal);
    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) =>
        _innerReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    public override char GetChar(int ordinal) => _innerReader.GetChar(ordinal);
    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) =>
        _innerReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    public override string GetDataTypeName(int ordinal) => _innerReader.GetDataTypeName(ordinal);
    public override DateTime GetDateTime(int ordinal) => _innerReader.GetDateTime(ordinal);
    public override decimal GetDecimal(int ordinal) => _innerReader.GetDecimal(ordinal);
    public override double GetDouble(int ordinal) => _innerReader.GetDouble(ordinal);
    public override Type GetFieldType(int ordinal) => _innerReader.GetFieldType(ordinal);
    public override float GetFloat(int ordinal) => _innerReader.GetFloat(ordinal);
    public override Guid GetGuid(int ordinal) => _innerReader.GetGuid(ordinal);
    public override short GetInt16(int ordinal) => _innerReader.GetInt16(ordinal);
    public override int GetInt32(int ordinal) => _innerReader.GetInt32(ordinal);
    public override long GetInt64(int ordinal) => _innerReader.GetInt64(ordinal);
    public override string GetName(int ordinal) => _innerReader.GetName(ordinal);
    public override int GetOrdinal(string name) => _innerReader.GetOrdinal(name);
    public override string GetString(int ordinal) => _innerReader.GetString(ordinal);
    public override object GetValue(int ordinal) => _innerReader.GetValue(ordinal);
    public override int GetValues(object[] values) => _innerReader.GetValues(values);
    public override bool IsDBNull(int ordinal) => _innerReader.IsDBNull(ordinal);
    public override bool NextResult() => _innerReader.NextResult();
    public override bool Read() => _innerReader.Read();
    public override DataTable GetSchemaTable() => _innerReader.GetSchemaTable();
    public override IEnumerator GetEnumerator() => new DbEnumerator(this);

    public override void Close()
    {
        _innerReader.Close();
        _connection.Close();
    }

    public override async Task<bool> ReadAsync(CancellationToken cancellationToken) =>
        await _innerReader.ReadAsync(cancellationToken).ConfigureAwait(false);

    public override async Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
        await _innerReader.NextResultAsync(cancellationToken).ConfigureAwait(false);

    public override async Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) =>
        await _innerReader.IsDBNullAsync(ordinal, cancellationToken).ConfigureAwait(false);

    public override async Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
        await _innerReader.GetFieldValueAsync<T>(ordinal, cancellationToken).ConfigureAwait(false);


#if !NETSTANDARD2_0
    public override async ValueTask DisposeAsync()
    {
        await _innerReader.DisposeAsync().ConfigureAwait(false);
        _connection.Close();
    }
#endif

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerReader.Dispose();
            _connection.Close();
        }
        base.Dispose(disposing);
    }
}
