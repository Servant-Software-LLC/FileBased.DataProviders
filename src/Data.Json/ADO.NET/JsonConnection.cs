namespace System.Data.JsonClient;
public class JsonConnection : FileConnection
{
    public JsonConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new JsonReader(this);

    }


    public override string FileExtension
    {
        get
        {
            return "json";
        }
    }

    public override IDbTransaction BeginTransaction()
    {
        return new JsonTransaction(this, default);
    }

    public override IDbTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();

    public override IDbCommand CreateCommand()
    {
        return new JsonCommand();
    }

   
}