using System.Diagnostics.CodeAnalysis;

namespace System.Data.FileClient;

public class FileDataReader : DbDataReader
{
    private readonly IEnumerator<FileQuery> queryParserEnumerator;
    private readonly FileReader fileReader;
    private readonly DataTable workingResultSet;
    private FileEnumerator currentFileEnumerator;
    private object?[]? currentDataRow = null;

    public FileDataReader(IEnumerable<FileQuery> queryParsers, FileReader fileReader)
    {
        if (queryParsers == null)
            throw new ArgumentNullException(nameof(queryParsers));

        queryParserEnumerator = queryParsers.GetEnumerator();
        this.fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));

        NextResult();
    }

    public override int Depth => 0;
    public override bool IsClosed => currentFileEnumerator == null;
    public override int RecordsAffected => -1;

    /// <summary>
    /// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.</returns>
    public override IEnumerator GetEnumerator() => new DbEnumerator(this);

    /// <summary>
    /// Gets a value that indicates whether this DbDataReader contains one or more rows.
    /// </summary>
    public override bool HasRows => currentDataRow != null;

    public override void Close() => Dispose();

    public override DataTable GetSchemaTable()
    {
        DataTable tempSchemaTable = new DataTable("SchemaTable");
        tempSchemaTable.Locale = Globalization.CultureInfo.InvariantCulture;

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

        if (workingResultSet.DataSet != null)
        {
            BaseCatalogName.DefaultValue = workingResultSet.DataSet.DataSetName;
        }

        BaseTableName.DefaultValue = workingResultSet.TableName;
        BaseTableNamespace.DefaultValue = workingResultSet.Namespace;
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

        foreach (string cl in currentFileEnumerator.Columns)
        {
            DataColumn dc = workingResultSet.Columns[cl]!;
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

        foreach (DataColumn key in workingResultSet.PrimaryKey)
        {
            tempSchemaTable.Rows[key.Ordinal][IsKeyColumn] = true;
        }

        tempSchemaTable.AcceptChanges();

        return tempSchemaTable;
    }

    public override bool NextResult()
    {
        if (!queryParserEnumerator.MoveNext())
            return false;

        //Create the DataTable which is our working resultset
        var queryParser = queryParserEnumerator.Current;
        var workingResultSet = fileReader.ReadFile(queryParser, true);
        if (workingResultSet == null)
            throw new ArgumentNullException(nameof(workingResultSet));

        var filter = queryParser!.Filter;

        currentFileEnumerator = new FileEnumerator(queryParser.GetColumnNames(), workingResultSet, filter);
        return true;
    }

    public override bool Read()
    {
        if (currentFileEnumerator.MoveNext())
        {
            currentDataRow = currentFileEnumerator.Current;
            return true;
        }

        return false;
    }

    public override int FieldCount => currentFileEnumerator.FieldCount;

    public override bool GetBoolean(int i) => GetValueAsType<bool>(i);

    public override byte GetByte(int i) => GetValueAsType<byte>(i);

    public override long GetBytes(int ordinal, long dataIndex, byte[]? buffer, int bufferIndex, int length)
    {
        byte[] tempBuffer;
        tempBuffer = (byte[])currentDataRow![ordinal]!;
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

    public override char GetChar(int i) => GetValueAsType<char>(i);

    public override long GetChars(int ordinal, long dataIndex, char[]? buffer, int bufferIndex, int length)
    {
        char[] tempBuffer;
            tempBuffer = (char[])currentDataRow![ordinal]!;
      
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


    public override string GetDataTypeName(int i) => GetValueAsType<string>(i).GetType().Name;
    public override DateTime GetDateTime(int i) => GetValueAsType<DateTime>(i);
    public override decimal GetDecimal(int i) => GetValueAsType<decimal>(i);
    public override double GetDouble(int i) => GetValueAsType<double>(i);
    public override Type GetFieldType(int i) => currentFileEnumerator.GetType(i);
    public override float GetFloat(int i) => GetValueAsType<float>(i);
    public override Guid GetGuid(int i) => GetValueAsType<Guid>(i);
    public override short GetInt16(int i) => GetValueAsType<short>(i);
    public override int GetInt32(int i) => GetValueAsType<int>(i);
    public override long GetInt64(int i) => GetValueAsType<long>(i);
    public override string GetName(int i) => currentFileEnumerator.GetName(i);
    public override int GetOrdinal(string name) => currentFileEnumerator.GetOrdinal(name);
    public override string GetString(int i) => GetValueAsType<string>(i);

    public override object GetValue(int i) => currentDataRow != null ? currentDataRow[i]!
                                        : throw new ArgumentNullException(nameof(currentDataRow));

    public override int GetValues(object[] values)
    {
        if (currentDataRow == null)
            return 0;

        Array.Copy(currentDataRow, values, currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
        return (currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
    }

    public override bool IsDBNull(int i) => currentDataRow is not null ? currentDataRow[i] == null
                                        : throw new ArgumentNullException(nameof(currentDataRow));

    protected new void Dispose() => currentDataRow = null;

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[string name]
    {
        get
        {
            int ordinal = GetOrdinal(name);
            return GetValue(ordinal);
        }
    }

    public T GetValueAsType<T>(int index) => (T)Convert.ChangeType(currentDataRow![index], typeof(T))!;


    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[int ordinal] => GetValue(ordinal);
}