using System.Diagnostics.CodeAnalysis;

namespace System.Data.JsonClient;

public class JsonDataReader : IDataReader
{
    private readonly JsonReader _reader;
    private readonly JsonConnection jsonConnection;
    private object?[] _currentDataRow;

    public JsonDataReader(JsonCommand command, IDbConnection jsonConnection)
    {
        this.jsonConnection = (JsonConnection)jsonConnection;
        _reader = new JsonReader(command,this.jsonConnection);
    }

    public int Depth => 0;
    public bool IsClosed => _reader == null;
    public int RecordsAffected => -1;

    public void Close()
    {
        Dispose();
    }

    public DataTable GetSchemaTable()
    {
        return GetSchemaTableFromDataTable(jsonConnection.JsonReader.DataTable);
    }
    internal DataTable GetSchemaTableFromDataTable(DataTable table)
    {
     
        DataTable tempSchemaTable = new DataTable("SchemaTable");
        tempSchemaTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

        DataColumn ColumnName = new DataColumn(SchemaTableColumn.ColumnName, typeof(string));
        DataColumn ColumnOrdinal = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int));
        DataColumn ColumnSize = new DataColumn(SchemaTableColumn.ColumnSize, typeof(int));
        DataColumn NumericPrecision = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short));
        DataColumn NumericScale = new DataColumn(SchemaTableColumn.NumericScale, typeof(short));
        DataColumn DataType = GetSystemTypeDataColumn();

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2111:ReflectionToDynamicallyAccessedMembers",
            Justification = "The problem is Type.TypeInitializer which requires constructors on the Type instance." +
                "In this case the TypeInitializer property is not accessed dynamically.")]
        static DataColumn GetSystemTypeDataColumn() =>
            new DataColumn(SchemaTableColumn.DataType, typeof(Type));

        DataColumn ProviderType = new DataColumn(SchemaTableColumn.ProviderType, typeof(int));
        DataColumn IsLong = new DataColumn(SchemaTableColumn.IsLong, typeof(bool));
        DataColumn AllowDBNull = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool));
        DataColumn IsReadOnly = new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
        DataColumn IsRowVersion = new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
        DataColumn IsUnique = new DataColumn(SchemaTableColumn.IsUnique, typeof(bool));
        DataColumn IsKeyColumn = new DataColumn(SchemaTableColumn.IsKey, typeof(bool));
        DataColumn IsAutoIncrement = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
        DataColumn BaseSchemaName = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string));
        DataColumn BaseCatalogName = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
        DataColumn BaseTableName = new DataColumn(SchemaTableColumn.BaseTableName, typeof(string));
        DataColumn BaseColumnName = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string));
        DataColumn AutoIncrementSeed = new DataColumn(SchemaTableOptionalColumn.AutoIncrementSeed, typeof(long));
        DataColumn AutoIncrementStep = new DataColumn(SchemaTableOptionalColumn.AutoIncrementStep, typeof(long));
        DataColumn DefaultValue = new DataColumn(SchemaTableOptionalColumn.DefaultValue, typeof(object));
        DataColumn Expression = new DataColumn(SchemaTableOptionalColumn.Expression, typeof(string));
        DataColumn ColumnMapping = new DataColumn(SchemaTableOptionalColumn.ColumnMapping, typeof(MappingType));
        DataColumn BaseTableNamespace = new DataColumn(SchemaTableOptionalColumn.BaseTableNamespace, typeof(string));
        DataColumn BaseColumnNamespace = new DataColumn(SchemaTableOptionalColumn.BaseColumnNamespace, typeof(string));

        ColumnSize.DefaultValue = -1;

        if (table.DataSet != null)
        {
            BaseCatalogName.DefaultValue = table.DataSet.DataSetName;
        }

        BaseTableName.DefaultValue = table.TableName;
        BaseTableNamespace.DefaultValue = table.Namespace;
        IsRowVersion.DefaultValue = false;
        IsLong.DefaultValue = false;
        IsReadOnly.DefaultValue = false;
        IsKeyColumn.DefaultValue = false;
        IsAutoIncrement.DefaultValue = false;
        AutoIncrementSeed.DefaultValue = 0;
        AutoIncrementStep.DefaultValue = 1;

        tempSchemaTable.Columns.Add(ColumnName);
        tempSchemaTable.Columns.Add(ColumnOrdinal);
        tempSchemaTable.Columns.Add(ColumnSize);
        tempSchemaTable.Columns.Add(NumericPrecision);
        tempSchemaTable.Columns.Add(NumericScale);
        tempSchemaTable.Columns.Add(DataType);
        tempSchemaTable.Columns.Add(ProviderType);
        tempSchemaTable.Columns.Add(IsLong);
        tempSchemaTable.Columns.Add(AllowDBNull);
        tempSchemaTable.Columns.Add(IsReadOnly);
        tempSchemaTable.Columns.Add(IsRowVersion);
        tempSchemaTable.Columns.Add(IsUnique);
        tempSchemaTable.Columns.Add(IsKeyColumn);
        tempSchemaTable.Columns.Add(IsAutoIncrement);
        tempSchemaTable.Columns.Add(BaseCatalogName);
        tempSchemaTable.Columns.Add(BaseSchemaName);

        // specific to datatablereader
        tempSchemaTable.Columns.Add(BaseTableName);
        tempSchemaTable.Columns.Add(BaseColumnName);
        tempSchemaTable.Columns.Add(AutoIncrementSeed);
        tempSchemaTable.Columns.Add(AutoIncrementStep);
        tempSchemaTable.Columns.Add(DefaultValue);
        tempSchemaTable.Columns.Add(Expression);
        tempSchemaTable.Columns.Add(ColumnMapping);
        tempSchemaTable.Columns.Add(BaseTableNamespace);
        tempSchemaTable.Columns.Add(BaseColumnNamespace);

        foreach (string cl in _reader.Columns)
        {
            DataColumn dc = table.Columns[cl]!;
            DataRow dr = tempSchemaTable.NewRow();

            dr[ColumnName] = dc.ColumnName;
            dr[ColumnOrdinal] = dc.Ordinal;
            dr[DataType] = dc.DataType;

            if (dc.DataType == typeof(string))
            {
                dr[ColumnSize] = dc.MaxLength;
            }

            dr[AllowDBNull] = dc.AllowDBNull;
            dr[IsReadOnly] = dc.ReadOnly;
            dr[IsUnique] = dc.Unique;

            if (dc.AutoIncrement)
            {
                dr[IsAutoIncrement] = true;
                dr[AutoIncrementSeed] = dc.AutoIncrementSeed;
                dr[AutoIncrementStep] = dc.AutoIncrementStep;
            }

            if (dc.DefaultValue != DBNull.Value)
            {
                dr[DefaultValue] = dc.DefaultValue;
            }
            dr[ColumnMapping] = dc.ColumnMapping;
            dr[BaseColumnName] = dc.ColumnName;
            dr[BaseColumnNamespace] = dc.Namespace;

            tempSchemaTable.Rows.Add(dr);
        }

        foreach (DataColumn key in table.PrimaryKey)
        {
            tempSchemaTable.Rows[key.Ordinal][IsKeyColumn] = true;
        }

        tempSchemaTable.AcceptChanges();

        return tempSchemaTable;
    }

    public bool NextResult()
    {
        return false;
    }

    public bool Read()
    {
        if (_reader.MoveNext())
        {
            _currentDataRow = _reader.Current;
            return true;
        }

        return false;
    }

    public int FieldCount => _reader.FieldCount;

    public bool GetBoolean(int i)
    {
        return GetValueAsType<bool>(i);
    }

    public byte GetByte(int i)
    {
        return GetValueAsType<byte>(i);
    }

    public long GetBytes(int ordinal, long dataIndex, byte[]? buffer, int bufferIndex, int length)
    {
        byte[] tempBuffer;
        tempBuffer = (byte[])_currentDataRow![ordinal]!;
        if (buffer == null)
        {
            return tempBuffer.Length;
        }

        int srcIndex = (int)dataIndex;
        int byteCount = Math.Min(tempBuffer.Length - srcIndex, length);
        if (srcIndex < 0)
        {
            throw new Exception("Invalid buffer index");
        }
        else if ((bufferIndex < 0) || (bufferIndex > 0 && bufferIndex >= buffer.Length))
        {
            throw new Exception("Invalid destination buffer index");

        }

        if (0 < byteCount)
        {
            Array.Copy(tempBuffer, dataIndex, buffer, bufferIndex, byteCount);
        }
        else if (length < 0)
        {
            throw new Exception("Invalid data length");
        }
        else
        {
            byteCount = 0;
        }
        return byteCount;

    }

    public char GetChar(int i)
    {
        return GetValueAsType<char>(i);
    }

    public long GetChars(int ordinal, long dataIndex, char[]? buffer, int bufferIndex, int length)
    {
        char[] tempBuffer;
            tempBuffer = (char[])_currentDataRow![ordinal]!;
      
        if (buffer == null)
        {
            return tempBuffer.Length;
        }
        int srcIndex = (int)dataIndex;
        int charCount = Math.Min(tempBuffer.Length - srcIndex, length);
        if (srcIndex < 0)
        {
            throw new Exception("Invalid buffer index");

        }
        else if ((bufferIndex < 0) || (bufferIndex > 0 && bufferIndex >= buffer.Length))
        {
            throw new Exception("Invalid destination buffer index");

        }

        if (0 < charCount)
        {
            Array.Copy(tempBuffer, dataIndex, buffer, bufferIndex, charCount);
        }
        else if (length < 0)
        {
            throw new Exception("Invalid data length");

        }
        else
        {
            charCount = 0;
        }
        return charCount;
    }


    public IDataReader GetData(int i)
    {
        throw new NotSupportedException();
    }

    public string GetDataTypeName(int i)
    {
        return GetValueAsType<string>(i).GetType().Name;
    }

    public DateTime GetDateTime(int i)
    {
        return GetValueAsType<DateTime>(i);
    }

    public decimal GetDecimal(int i)
    {
        return GetValueAsType<decimal>(i);
    }

    public double GetDouble(int i)
    {
        return GetValueAsType<double>(i);
    }

    public Type GetFieldType(int i)
    {
        return _reader.GetType(i);
    }

    public float GetFloat(int i)
    {
        return GetValueAsType<float>(i);
    }

    public Guid GetGuid(int i)
    {
        return GetValueAsType<Guid>(i);
    }

    public short GetInt16(int i)
    {
        return GetValueAsType<short>(i);
    }

    public int GetInt32(int i)
    {
        return GetValueAsType<int>(i);
    }

    public long GetInt64(int i)
    {
        return GetValueAsType<long>(i);
    }

    public string GetName(int i)
    {
        return _reader.GetName(i);
    }

    public int GetOrdinal(string name)
    {
        return _reader.GetOrdinal(name);
    }
    public string GetString(int i)
    {
        return GetValueAsType<string>(i);
    }
    public object GetValue(int i)
    {
        return _currentDataRow[i]!;
    }
    public int GetValues(object[] values)
    {
        Array.Copy(_currentDataRow, values, _currentDataRow.Length > values.Length ? values.Length : _currentDataRow.Length);
        return (_currentDataRow.Length > values.Length ? values.Length : _currentDataRow.Length);
    }
    public bool IsDBNull(int i)
    {
        return _currentDataRow[i]!=null;
    }
    public void Dispose()
    {
        _currentDataRow = null;
    }
    public object this[string name]
    {
        get
        {
            int ordinal = GetOrdinal(name);
            return GetValue(ordinal);
        }
    }
    public T GetValueAsType<T>(int index)
    {
        return (T)Convert.ChangeType(_currentDataRow![index], typeof(T))!;
    }


    public object this[int i]
    {
        get
        {
            return GetValue(i);
        }
    }
}