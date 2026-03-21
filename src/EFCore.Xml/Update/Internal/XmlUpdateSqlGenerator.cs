using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Xml.Update.Internal;

public class XmlUpdateSqlGenerator : UpdateSqlGenerator
{
    public XmlUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }

    public override string GenerateNextSequenceValueOperation(string name, string schema)
        => throw new NotSupportedException("XML ADO.NET provider does not support sequences.See http://go.microsoft.com/fwlink/?LinkId=723262 for more information.");

}
