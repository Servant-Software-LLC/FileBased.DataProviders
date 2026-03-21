using EFCore.Common.Update.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Csv.Update.Internal;

public class CsvUpdateSqlGenerator : FileUpdateSqlGeneratorBase
{
    public CsvUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }
}
