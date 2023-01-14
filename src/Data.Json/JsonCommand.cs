using System.Data;

namespace Data.Json;

public class JsonCommand : IDbCommand
{
    public JsonCommand()
    {

    }

    public JsonCommand(string command)
    {

    }

    public JsonCommand(string cmdText, JsonConnection connection)
    {

    }

    public JsonCommand(string cmdText, JsonConnection connection, JsonTransaction transaction)
    {

    }

    public string CommandText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int CommandTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public CommandType CommandType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IDbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public JsonParameterCollection Parameters => throw new NotImplementedException();

    IDataParameterCollection IDbCommand.Parameters => throw new NotImplementedException();

    public IDbTransaction? Transaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public UpdateRowSource UpdatedRowSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public IDbDataParameter CreateParameter()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public int ExecuteNonQuery()
    {
        throw new NotImplementedException();
    }

    public IDataReader ExecuteReader()
    {
        throw new NotImplementedException();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        throw new NotImplementedException();
    }

    public object? ExecuteScalar()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        throw new NotImplementedException();
    }
}
