using Irony.Parsing;

namespace Data.Common.FileQuery;

public class FileCreateDatabaseQuery<TFileParameter> : FileAdminQuery<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public FileCreateDatabaseQuery(ParseTreeNode node, FileCommand<TFileParameter> fileCommand)
        : base(node, fileCommand)
    {
        
    }

    public override bool Execute()
    {
        var connection = fileCommand.FileConnection;
        
        if (string.IsNullOrEmpty(Database)) 
            throw new ArgumentNullException(nameof(Database), $"The database must be specified!");

        var pathType = FileConnection<TFileParameter>.GetPathType(Database, fileCommand.FileConnection.FileExtension);

        switch (pathType)
        {
            case PathType.File:

                //If the database file already exists.
                if (File.Exists(Database))
                    return false;

                //Create the provider-specific file for an empty database.
                connection.CreateFileAsDatabase(Database);
                break;

            case PathType.Directory:

                //If the database folder already exists.
                if (Directory.Exists(Database))
                    return false;

                //Create the folder which represents an empty database for any of the providers.
                Directory.CreateDirectory(Database); 
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Database), $"The database provided in the CREATE DATABASE command is not recognized as either a file or folder path.");
        }

        return true;
    }
}
