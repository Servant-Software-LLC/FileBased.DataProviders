namespace Data.Common.Interfaces;

public interface IFileDataAdapter : IDbDataAdapter, IDisposable
{
    int Fill(DataTable dataTable);
}
