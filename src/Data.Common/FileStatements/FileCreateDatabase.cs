using Data.Common.DataSource;
using Irony.Parsing;

namespace Data.Common.FileStatements;

public class FileCreateDatabase<TFileParameter> : FileAdminStatement<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public FileCreateDatabase(ParseTreeNode node, FileCommand<TFileParameter> fileCommand)
        : base(node, fileCommand)
    {
        
    }

    public override bool Execute()
    {
        var connection = fileCommand.FileConnection;
        return Execute(connection, Database);
    }

    internal static bool Execute(FileConnection<TFileParameter> fileConnection, string database)
    {
        if (string.IsNullOrEmpty(database))
            throw new ArgumentNullException(nameof(database), $"The database must be specified!");

        var pathType = GetDataSourceType(database, fileConnection.FileExtension);

        switch (pathType)
        {
            case DataSourceType.File:

                //If the database file already exists.
                if (File.Exists(database))
                    return false;

                //Create the provider-specific file for an empty database.
                fileConnection.CreateFileAsDatabase(database);
                break;

            case DataSourceType.Directory:

                //If the database folder already exists.
                if (Directory.Exists(database))
                    return false;

                //Create the folder which represents an empty database for any of the providers.
                Directory.CreateDirectory(database);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(database), $"The database, {database}, is not recognized as either a file or folder path.");
        }

        return true;
    }

}
