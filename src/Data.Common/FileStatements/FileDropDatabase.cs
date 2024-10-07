using Data.Common.DataSource;
using Irony.Parsing;

namespace Data.Common.FileStatements;

public class FileDropDatabase<TFileParameter> : FileAdminStatement<TFileParameter>
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public FileDropDatabase(ParseTreeNode node, FileCommand<TFileParameter> fileCommand)
        : base(node, fileCommand)
    {
        
    }

    public override bool Execute()
    {
        var connection = fileCommand.FileConnection;

        if (string.IsNullOrEmpty(Database))
            throw new ArgumentNullException(nameof(Database), $"The database must be specified!");

        var providerFileExtension = fileCommand.FileConnection.FileExtension;
        var pathType = GetDataSourceType(Database, providerFileExtension);

        switch (pathType)
        {
            case DataSourceType.File:

                //If the database file doesn't exists, just return that nothing was done.
                if (!File.Exists(Database))
                    return false;

                //Delete the provider-specific file.  GetPathType() above ensures that the file extension is the correct one for this provider.
                File.Delete(Database);

                break;

            case DataSourceType.Directory:

                //If the database folder doesn't exist, just return that nothing was done.
                if (!Directory.Exists(Database))
                    return false;

                //Before we attempt this destructive operation, we need to be very protective by making some checks on the contents of the folder.
                //Check that there are no sub-folders.
                if (Directory.GetDirectories(Database).Length > 0)
                    throw new InvalidOperationException($"Unable to DROP DATABASE {Database} because it contains sub-folders.");
                var filesInFolder = Directory.GetFiles(Database);
                if (filesInFolder.Any(fileName => string.Compare(Path.GetExtension(fileName), providerFileExtension, true) != 0))
                    throw new InvalidOperationException($"Unable to DROP DATABASE {Database} because it contains files that are not of the {providerFileExtension} extension.");

                //Delete the folder which represents the database.
                foreach(var file in filesInFolder)
                    File.Delete(file);
                Directory.Delete(Database);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Database), $"The database provided in the CREATE DATABASE command is not recognized as either a file or folder path.");
        }

        return true;
    }

}
