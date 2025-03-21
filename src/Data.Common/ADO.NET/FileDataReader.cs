﻿using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;

/// <summary>
/// Represents an abstract class for reading file data.
/// </summary>
public abstract class FileDataReader : DbDataReader
{
    private ILogger<FileDataReader> log => loggerServices.CreateLogger<FileDataReader>();

    private readonly IEnumerator<FileStatement> statementEnumerator;
    private readonly FileReader fileReader;
    private readonly TransactionScopedRows transactionScopedRows;
    private readonly Func<FileStatement, FileWriter> createWriter;
    private readonly LoggerServices loggerServices;
    private Result previousWriteResult;

    private Result result;


    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataReader"/> class.
    /// </summary>
    /// <param name="fileStatements">The file statements to be executed.</param>
    /// <param name="fileReader">The file reader.</param>
    /// <param name="transactionScopedRows">The transaction scoped rows.</param>
    /// <param name="createWriter">The function to create a file writer.</param>
    /// <param name="loggerServices">The logger services.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required parameters are null.</exception>
    protected FileDataReader(IEnumerable<FileStatement> fileStatements, 
                             FileReader fileReader,
                             TransactionScopedRows transactionScopedRows,
                             Func<FileStatement, FileWriter> createWriter,
                             LoggerServices loggerServices)
    {
        if (fileStatements == null)
            throw new ArgumentNullException(nameof(fileStatements));

        statementEnumerator = fileStatements.GetEnumerator();
        this.fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        this.transactionScopedRows = transactionScopedRows;  //Note:  This value may be null.
        this.createWriter = createWriter ?? throw new ArgumentNullException(nameof(createWriter));
        this.loggerServices = loggerServices ?? throw new ArgumentNullException(nameof(loggerServices));

        NextResult();
    }

    /// <summary>
    /// Gets the depth of nesting for the current row.
    /// </summary>
    public override int Depth => 0;


    /// <summary>
    /// Gets a value indicating whether the data reader is closed.
    /// </summary>
    public override bool IsClosed => result.IsClosed;

    /// <summary>
    /// Copying SQL Server ADO.NET provider's behavior here.  It retains the RowsAffected value
    /// from the last executed non-query command when moving to the next result set using NextResult().
    /// </summary>
    public override int RecordsAffected
    {
        get
        {
            var returnValue = previousWriteResult != null ? previousWriteResult.RecordsAffected : -1;
            log.LogDebug($"{GetType()}.{nameof(RecordsAffected)} = {returnValue}");
            return returnValue;
        }
    }

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

    /// <summary>
    /// Advances to the next result, when reading the results of batch SQL statements.
    /// </summary>
    /// <returns>True if there are more rows; otherwise, false.</returns>
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

                //Log the executing statement as well as its parameters (if any)
                log.LogInformation($"{GetType()}.{nameof(NextResult)}(). Executing statement: {fileStatement.Statement}");

                result = new Result(fileStatement, fileReader, createWriter, previousWriteResult, transactionScopedRows, log);

                //Save the last write statement that we executed to evaluate built-in functions.
                isSelectStatement = fileStatement is FileSelect;
                if (!isSelectStatement)
                {
                    previousWriteResult = result;
                    log.LogDebug($"Saving previous WriteResult. RowsAffected: {previousWriteResult.RecordsAffected}. Statement: {previousWriteResult.Statement}");

                    //If this result has an addition row from an INSERT that is in the middle of a transaction..
                    if (result.TransactionScopedRow.HasValue)
                    {
                        var tableName = result.TransactionScopedRow.Value.TableName;
                        if (!transactionScopedRows.TryGetValue(tableName, out List<DataRow> additionalRows))
                        {
                            additionalRows = new List<DataRow>();
                            transactionScopedRows[tableName] = additionalRows;
                        }

                        additionalRows.Add(result.TransactionScopedRow.Value.Row);
                    }

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

    /// <summary>
    /// Gets the schema table for the current result set.
    /// </summary>
    /// <returns>A <see cref="DataTable"/> that describes the column metadata of the current result set.</returns>
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

#if !NETSTANDARD2_0
        [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("ReflectionAnalysis", "IL2111:ReflectionToDynamicallyAccessedMembers",
            Justification = "The problem is Type.TypeInitializer which requires constructors on the Type instance." +
                "In this case the TypeInitializer property is not accessed dynamically.")]
#endif
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

        //TODO: Probably can live without these
        //if (workingResultSet.DataSet != null)
        //{
        //    BaseCatalogName.DefaultValue = workingResultSet.DataSet.DataSetName;
        //}
        //BaseTableNamespace.DefaultValue = workingResultSet.Namespace;

        BaseTableName.DefaultValue = workingResultSet.TableName;
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

        foreach (DataColumn dataColumn in result.FileEnumerator.Columns)
        {
            DataRow dataRow = tempSchemaTable.NewRow();

            dataRow[ColumnName] = dataColumn.ColumnName;
            dataRow[ColumnOrdinal] = dataColumn.Ordinal;
            dataRow[DataType] = dataColumn.DataType;

            if (dataColumn.DataType == typeof(string))
            {
                dataRow[ColumnSize] = dataColumn.MaxLength;
            }

            dataRow[AllowDBNull] = dataColumn.AllowDBNull;
            dataRow[IsReadOnly] = dataColumn.ReadOnly;
            dataRow[IsUnique] = dataColumn.Unique;

            if (dataColumn.AutoIncrement)
            {
                dataRow[IsAutoIncrement] = true;
                dataRow[AutoIncrementSeed] = dataColumn.AutoIncrementSeed;
                dataRow[AutoIncrementStep] = dataColumn.AutoIncrementStep;
            }

            if (dataColumn.DefaultValue != DBNull.Value)
            {
                dataRow[DefaultValue] = dataColumn.DefaultValue;
            }
            dataRow[ColumnMapping] = dataColumn.ColumnMapping;
            dataRow[BaseColumnName] = dataColumn.ColumnName;
            dataRow[BaseColumnNamespace] = dataColumn.Namespace;

            tempSchemaTable.Rows.Add(dataRow);
        }

        //TODO:  Primary keys aren't considered for now.
        //foreach (DataColumn key in workingResultSet.PrimaryKey)
        //{
        //    tempSchemaTable.Rows[key.Ordinal][IsKeyColumn] = true;
        //}

        tempSchemaTable.AcceptChanges();

        return tempSchemaTable;
    }

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns>true if there are more rows; otherwise, false.</returns>
    public override bool Read()
    {
        log.LogDebug($"{GetType()}.{nameof(Read)}() called.");
        return result.Read();
    }

    /// <summary>
    /// Gets the number of columns in the current row.
    /// </summary>
    public override int FieldCount
    {
        get
        {
            log.LogDebug($"{GetType()}.{nameof(FieldCount)}() getter called.");
            return result.FieldCount;
        }
    }

    /// <summary>
    /// Gets the value of the specified column as a boolean.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override bool GetBoolean(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetBoolean)}() called.");
        return GetValueAsType<bool>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as a byte.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override byte GetByte(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetByte)}() called.");
        return GetValueAsType<byte>(i);
    }

    /// <summary>
    /// Reads a stream of bytes from the specified column, starting at location indicated by dataIndex, into the buffer as an array, starting at the given buffer index.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataIndex">The index within the row from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
    /// <param name="bufferIndex">The index for buffer to start the read operation.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The actual number of bytes read.</returns>
    public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
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

    /// <summary>
    /// Gets the value of the specified column as a char.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override char GetChar(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetChar)}() called.");
        return GetValueAsType<char>(i);
    }

    /// <summary>
    /// Reads a stream of characters from the specified column, starting at location indicated by dataIndex, into the buffer as an array, starting at the given buffer index.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataIndex">The index within the row from which to start the read operation.</param>
    /// <param name="buffer">The buffer into which to read the stream of characters.</param>
    /// <param name="bufferIndex">The index for buffer to start the read operation.</param>
    /// <param name="length">The number of characters to read.</param>
    /// <returns>The actual number of characters read.</returns>
    public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
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


    /// <summary>
    /// Gets the data type name of the specified column.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The data type name of the column.</returns>
    public override string GetDataTypeName(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDataTypeName)}() called.");
        return GetValueAsType<string>(i).GetType().Name;
    }

    /// <summary>
    /// Gets the value of the specified column as a DateTime.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override DateTime GetDateTime(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDateTime)}() called.");
        return GetValueAsType<DateTime>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as a decimal.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override decimal GetDecimal(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDecimal)}() called.");
        return GetValueAsType<decimal>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as a double.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override double GetDouble(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetDouble)}() called.");
        return GetValueAsType<double>(i);
    }

    /// <summary>
    /// Gets the Type that is the data type of the object.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The Type that is the data type of the object.</returns>
    public override Type GetFieldType(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetFieldType)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetFieldType(i);
    }

    /// <summary>
    /// Gets the value of the specified column as a float.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override float GetFloat(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetFloat)}() called.");
        return GetValueAsType<float>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as a Guid.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override Guid GetGuid(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetGuid)}() called.");
        return GetValueAsType<Guid>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as an Int16.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override short GetInt16(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt16)}() called.");
        return GetValueAsType<short>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as an Int32.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override int GetInt32(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt32)}() called.");
        return GetValueAsType<int>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as an Int64.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override long GetInt64(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetInt64)}() called.");
        return GetValueAsType<long>(i);
    }

    /// <summary>
    /// Gets the column name from the specified index.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The column name.</returns>
    public override string GetName(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetName)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetName(i);
    }

    /// <summary>
    /// Gets the column ordinal given the name of the column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The zero-based column ordinal.</returns>
    public override int GetOrdinal(string name)
    {
        log.LogDebug($"{GetType()}.{nameof(GetOrdinal)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.FileEnumerator.GetOrdinal(name);
    }

    /// <summary>
    /// Gets the value of the specified column as a string.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override string GetString(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetString)}() called.");

        return GetValueAsType<string>(i);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of Object.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>The value of the column.</returns>
    public override object GetValue(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValue)}() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return result.CurrentDataRow != null ? result.CurrentDataRow[i]!
                                             : throw new ArgumentNullException(nameof(result.CurrentDataRow));
    }

    /// <summary>
    /// Populates an array of objects with the column values of the current row.
    /// </summary>
    /// <param name="values">An array of Object to copy the attribute columns into.</param>
    /// <returns>The number of instances of Object in the array.</returns>
    public override int GetValues(object[] values)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValues)}() called.");

        var currentDataRow = result.CurrentDataRow;
        if (currentDataRow == null)
            return 0;

        Array.Copy(currentDataRow, values, currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
        return (currentDataRow.Length > values.Length ? values.Length : currentDataRow.Length);
    }

    /// <summary>
    /// Determines if the specified column is null.
    /// </summary>
    /// <param name="i">The zero-based column ordinal.</param>
    /// <returns>true if the specified column is null; otherwise, false.</returns>
    public override bool IsDBNull(int i)
    {
        log.LogDebug($"{GetType()}.{nameof(IsDBNull)}() called.");

        var currentDataRow = result.CurrentDataRow;

        return currentDataRow is not null ? currentDataRow[i].GetType() == typeof(DBNull)
                                          : throw new ArgumentNullException(nameof(currentDataRow));
    }

    /// <summary>
    /// Gets the value of the specified column and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="index">The zero-based column ordinal.</param>
    /// <returns>The value of the column converted to the specified type.</returns>
    public T GetValueAsType<T>(int index)
    {
        log.LogDebug($"{GetType()}.{nameof(GetValueAsType)}<{typeof(T).Name}>() called.");

        if (result.WorkingResultSet == null)
            throw new Exception($"Unable to read value.  SQL statement did not yield any resultset.  Statement: {result.Statement}");

        return (T)Convert.ChangeType(result.CurrentDataRow![index], typeof(T))!;
    }

    /// <summary>
    /// Disposes of the resources used by this instance.
    /// </summary>
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