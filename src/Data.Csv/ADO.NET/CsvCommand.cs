using Data.Common.Utils;
using Data.Csv.CsvIO.Create;

namespace System.Data.CsvClient;

public class CsvCommand : FileCommand<CsvParameter>
{
    public CsvCommand()
    {
    }

    public CsvCommand(string command) : base(command)
    {
    }

    public CsvCommand(CsvConnection connection) : base(connection)
    {
    }

    public CsvCommand(string cmdText, CsvConnection connection) : base(cmdText, connection)
    {
    }

    public CsvCommand(string cmdText, CsvConnection connection, CsvTransaction transaction) 
        : base(cmdText, connection, transaction)
    {
    }

    public override CsvParameter CreateParameter() => new();
    public override CsvParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    public override CsvDataAdapter CreateAdapter() => new(this);

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new CsvDelete(deleteStatement, (CsvConnection)Connection!, this),
        FileInsert insertStatement => new CsvInsert(insertStatement, (CsvConnection)Connection!, this),
        FileUpdate updateStatement => new CsvUpdate(updateStatement, (CsvConnection)Connection!, this),
        FileCreateTable createTableStatement => new CsvCreateTable(createTableStatement, (CsvConnection)Connection!, this),

        _ => throw new InvalidOperationException($"Cannot create writer for query {fileStatement.GetType()}.")
    };

    protected override CsvDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) => 
        new(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

}