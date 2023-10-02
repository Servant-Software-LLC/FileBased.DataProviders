using Data.Common.Utils;

namespace Data.Common.Interfaces;

public interface IFileTransaction : IDbTransaction
{
    bool TransactionDone { get; }
    List<FileWriter> Writers { get; }
    TransactionScopedRows TransactionScopedRows { get; }
}
