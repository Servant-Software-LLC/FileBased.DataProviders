using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Csv.Update.Internal;

public class CsvUpdateSqlGenerator : UpdateSqlGenerator
{
    public CsvUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }

    public override string GenerateNextSequenceValueOperation(string name, string schema)
        => throw new NotSupportedException("CSV ADO.NET provider does not support sequences.See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.");

}
