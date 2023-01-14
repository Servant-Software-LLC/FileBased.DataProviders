using System.Data;

namespace Data.Json;

public class JsonTransaction : IDbTransaction
{
    public IDbConnection? Connection => throw new NotImplementedException();

    public IsolationLevel IsolationLevel => throw new NotImplementedException();

    public void Commit()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }
}
