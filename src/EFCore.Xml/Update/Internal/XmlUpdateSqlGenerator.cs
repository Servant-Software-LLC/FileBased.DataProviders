using EFCore.Common.Update.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Xml.Update.Internal;

public class XmlUpdateSqlGenerator : FileUpdateSqlGeneratorBase
{
    public XmlUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
    : base(dependencies)
    {
    }
}
