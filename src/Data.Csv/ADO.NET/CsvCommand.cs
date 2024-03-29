﻿using Data.Common.Utils;
using Data.Csv.CsvIO.Create;

namespace System.Data.CsvClient;


/// <summary>
/// Represents a command for CSV operations.
/// </summary>
/// <inheritdoc/>
public class CsvCommand : FileCommand<CsvParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommand"/> class.
    /// </summary>
    public CsvCommand() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommand"/> class with the specified command text.
    /// </summary>
    /// <param name="command">The text of the command.</param>
    public CsvCommand(string command) : base(command) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommand"/> class with the specified connection.
    /// </summary>
    /// <param name="connection">The <see cref="CsvConnection"/> to use.</param>
    public CsvCommand(CsvConnection connection) : base(connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommand"/> class with the specified command text and connection.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <param name="connection">The <see cref="CsvConnection"/> to use.</param>
    public CsvCommand(string cmdText, CsvConnection connection) : base(cmdText, connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvCommand"/> class with the specified command text, connection, and transaction.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <param name="connection">The <see cref="CsvConnection"/> to use.</param>
    /// <param name="transaction">The <see cref="CsvTransaction"/> to use.</param>
    public CsvCommand(string cmdText, CsvConnection connection, CsvTransaction transaction)
        : base(cmdText, connection, transaction) { }

    /// <inheritdoc/>
    public override CsvParameter CreateParameter() => new();

    /// <inheritdoc/>
    public override CsvParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    /// <inheritdoc/>
    public override CsvDataAdapter CreateAdapter() => new(this);

    /// <inheritdoc />
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new CsvDelete(deleteStatement, (CsvConnection)Connection!, this),
        FileInsert insertStatement => new CsvInsert(insertStatement, (CsvConnection)Connection!, this),
        FileUpdate updateStatement => new CsvUpdate(updateStatement, (CsvConnection)Connection!, this),
        FileCreateTable createTableStatement => new CsvCreateTable(createTableStatement, (CsvConnection)Connection!, this),

        _ => throw new InvalidOperationException($"Cannot create writer for query {fileStatement.GetType()}.")
    };

    /// <inheritdoc />
    protected override CsvDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

}
