namespace Data.Common.Interfaces;

internal interface IFileConnectionInternal : IFileConnection
{
    Func<FileStatement, IDataSetWriter> CreateDataSetWriter { get; }
}
