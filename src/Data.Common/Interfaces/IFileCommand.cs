namespace Data.Common.Interfaces;

public interface IFileCommand : IDbCommand
{
    IFileConnection FileConnection { get; }
    IFileTransaction FileTransaction { get; }
    new DbParameterCollection Parameters { get; }
}
