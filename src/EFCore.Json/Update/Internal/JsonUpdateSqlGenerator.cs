using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Json.Update.Internal;

public class JsonUpdateSqlGenerator : UpdateSqlGenerator
{
    public JsonUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }

    public override string GenerateNextSequenceValueOperation(string name, string schema)
        => throw new NotSupportedException("JSON ADO.NET provider does not support sequences.See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.");

}
