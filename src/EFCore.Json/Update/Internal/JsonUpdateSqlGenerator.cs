using EFCore.Common.Update.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Json.Update.Internal;

public class JsonUpdateSqlGenerator : FileUpdateSqlGeneratorBase
{
    public JsonUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }
}
