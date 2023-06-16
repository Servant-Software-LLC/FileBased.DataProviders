using EFCore.Common.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.XmlClient;

namespace EFCore.Xml.Scaffolding.Internal;

public class XmlDatabaseModelFactory : FileDatabaseModelFactory
{
    public XmlDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {

    }

    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new XmlConnection(connectionString);
        return Create(connection, options);
    }

}
