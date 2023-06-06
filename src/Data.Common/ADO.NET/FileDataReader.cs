using Data.Common.FileStatements;
using Data.Common.Utils;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.FileClient;

public abstract class FileDataReader : DbDataReader
{
    private ILogger<FileDataReader> log => loggerServices.CreateLogger<FileDataReader>();

    private readonly IEnumerator<FileStatement> statementEnumerator;
    private readonly FileReader fileReader;
    private readonly Func<FileStatement, FileWriter> createWriter;
    private readonly LoggerServices loggerServices;
    private Result previousWriteResult;
    private Result result;
    
    protected FileDataReader(IEnumerable<FileStatement> fileStatements, 
                             FileReader fileReader, 
                             Func<FileStatement, FileWriter> createWriter,
                             LoggerServices loggerServices)
    {
        if (fileStatements == null)
            throw new ArgumentNullException(nameof(fileStatements));

        statementEnumerator = fileStatements.GetEnumerator();
        this.fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        this.createWriter = createWriter ?? throw new ArgumentNullException(nameof(createWriter));
        this.loggerServices = loggerServices ?? throw new ArgumentNullException(nameof(loggerServices));

        NextResult();
    }

    public override int Depth => 0;
    public override bool IsClosed => result.IsClosed;

    //Copying SQL Server ADO.NET provider's behavior here.  It retains the RowsAffected value
    //from the last executed non-query command when moving to the next result set using NextResult().
    public override int RecordsAffected => previousWriteResult != null ? previousWriteResult.RecordsAffected : -1;

    /// <summary>
    /// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.</returns>
    public override IEnumerator GetEnumerator() => new DbEnumerator(this);

    /// <summary>
    /// Gets a value that indicates whether this DbDataReader contains one or more rows.
    /// </summary>
    public override bool HasRows => result.HasRows;

    public override void Close() => Dispose();

    public override bool NextResult()
    {
        log.LogInformation($"{GetType()}.{nameof(NextResult)}() called.");

        try
        {
            bool isSelectStatement;
            do
            {
                if (!statementEnumerator.MoveNext())
                {
                    log.LogInformation($"{GetType()}.{nameof(NextResult)}(). No more results to enumerate.");
                    return false;
                }

                //Create the DataTable which is our working resultset
                var fileStatement = statementEnumerator.Current;

                log.LogInformation($"{GetType()}.{nameof(NextResult)}(). Executing statement: {fileStatement.Statement}");
                result = new Result(fileStatement, fileReader, createWriter, previousWriteResult, log);

                //Save the last write statement that we executed to evaluate built-in functions.
                isSelectStatement = fileStatement is FileSelect;
                if (!isSelectStatement)
                {
                    previousWriteResult = result;
                    log.LogDebug($"Saving previous WriteResult. RowsAffected: {previousWriteResult.RecordsAffected}. Statement: {previousWriteResult.Statement}");
                }

            } while (!isSelectStatement);

            return true;
        }
        catch (Exception ex)
        {
            log.LogError($"{GetType()}.{nameof(NextResult)}(). Exception occurred. {ex}");
            throw;
        }
    }

    public override DataTable GetSchemaTable()
    {
        log.LogDebug($"{GetType()}.{nameof(GetSchemaTable)}() called.");

        var workingResultSet = result.WorkingResultSet;
        if (workingResultSet == null)
            return null;


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

        foreach (string cl in result.FileEnumerator.Columns)
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

    public override bool Read()
    {
        log.LogDebug($"{GetType()}.{nameof(Read)}() called.");
        return result.Read();
    }

    public override int FieldCount
    {
        get
        {
            log.LogDebug($"{GetType()}.{nameof(FieldCount)}() getter called.");
            return result.FieldCount;
        }
    }

    public override bool GetBoolean(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetBoolean)}() called.");
        return GetValueAsType<bool>(i);
    }

    public override byte GetByte(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetByte)}() called.");
        return GetValueAsType<byte>(i);
    }

    public override long GetBytes(int ordinal, long dataIndex, byte[]? buffer, int bufferIndex, int length)
    {
        log.LogDebug($"{GetType()}.{nameof(GetBytes)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        byte[] tempBuffer;
        tempBuffer = (byte[])result.CurrentDataRow![ordinal]!;
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

    public override char GetChar(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetChar)}() called.");
        return GetValueAsType<char>(i);
    }

    public override long GetChars(int ordinal, long dataIndex, char[]? buffer, int bufferIndex, int length)
    {
        log.LogDebug($"{GetType()}.{nameof(GetChars)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        char[] tempBuffer;
            tempBuffer = (char[])result.CurrentDataRow![ordinal]!;
      
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


    public override string GetDataTypeName(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDataTypeName)}() called.");
        return GetValueAsType<string>(i).GetType().Name;
    }

    public override DateTime GetDateTime(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDateTime)}() called.");
        return GetValueAsType<DateTime>(i);
    }

    public override decimal GetDecimal(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDecimal)}() called.");
        return GetValueAsType<decimal>(i);
    }

    public override double GetDouble(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDouble)}() called.");
        return GetValueAsType<double>(i);
    }

    public override Type GetFieldType(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetFieldType)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetType(i);
    }
    public override float GetFloat(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetFloat)}() called.");
        return GetValueAsType<float>(i);
    }

    public override Guid GetGuid(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetGuid)}() called.");
        return GetValueAsType<Guid>(i);
    }

    public override short GetInt16(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt16)}() called.");
        return GetValueAsType<short>(i);
    }

    public override int GetInt32(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt32)}() called.");
        return GetValueAsType<int>(i);
    }

    public override long GetInt64(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt64)}() called.");
        return GetValueAsType<long>(i);
    }

    public override string GetName(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetName)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetName(i);
    }

    public override int GetOrdinal(string name)
    {
        log.LogDebug($"{GetType()}.{nameof(GetOrdinal)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetOrdinal(name);
    }

    public override string GetString(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetString)}() called.");

        return GetValueAsType<string>(i);
    }

    public override object GetValue(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValue)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.CurrentDataRow != null ? result.CurrentDataRow[i]!
                                             : throw new ArgumentNullException(nameof(result.CurrentDataRow));
    }

    public override int GetValues(object[] values)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValues)}() called.");

        var currentDataRow = result.CurrentDataRow;
        if (currentDataRow == null)
            return 0;

        Array.Copy(currentDataRow, values, currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
        return (currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
    }

    public override bool IsDBNull(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(IsDBNull)}() called.");

        var currentDataRow = result.CurrentDataRow;

        return currentDataRow is not null ? currentDataRow[i] == null
                                          : throw new ArgumentNullException(nameof(currentDataRow));
    }

    public T GetValueAsType<T>(int index)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValueAsType)}<{typeof(T).Name}>() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return (T)Convert.ChangeType(result.CurrentDataRow![index], typeof(T))!;
    }

    protected new void Dispose()
    {
        log.LogDebug($"{GetType()}.{nameof(Dispose)}() called.");
        fileReader.Dispose();
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[string name]
    {
        get
        {
            log.LogDebug($"{GetType()}.this[] called. Name = {name}");

            int ordinal = GetOrdinal(name);
            return GetValue(ordinal);
        }
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[int ordinal]
    {
        get
        {
            log.LogDebug($"{GetType()}.this[] called. Ordinal = {ordinal}");
            return GetValue(ordinal);
        }
    }
}