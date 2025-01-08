namespace Data.Common.FileException;

public abstract class InvalidFileException : Exception
{
    public InvalidFileException(string message)
      : base(message)
    {
    }
}
