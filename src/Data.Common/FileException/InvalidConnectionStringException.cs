namespace Data.Common.FileException;

public class InvalidConnectionStringException : Exception
{
    public InvalidConnectionStringException(string pathToDatabase) : base(GetMessage(pathToDatabase))
    {
    }

    private static string GetMessage(string pathToDatabase) => $"Database file or directory '{pathToDatabase}' not found";

}

