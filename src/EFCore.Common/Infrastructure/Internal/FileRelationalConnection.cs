using Data.Common.DataSource;
using EFCore.Csv.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Common.Infrastructure.Internal;

public abstract class FileRelationalConnection : RelationalConnection
{
    protected FileRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
        var fileOptions = FileOptionsExtension.ExtractFileOptions(dependencies.ContextOptions);

        if (fileOptions.DataSourceProvider != null)
            DataSourceProvider = fileOptions.DataSourceProvider;
    }

    protected IDataSourceProvider DataSourceProvider { get; set; }
}
