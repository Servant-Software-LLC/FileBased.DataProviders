using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Csv.Scaffolding.Internal;

public class CsvDatabaseModelFactory : IDatabaseModelFactory
{
    public DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        // Implementation goes here
    }

    public DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
    {
        // Implementation goes here
    }
}