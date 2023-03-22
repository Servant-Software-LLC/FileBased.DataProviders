namespace Data.Tests.Common.Utils;

public class ConnectionString
{
    private const string dataSource = "Data Source";

    readonly string dataSourceValue;
    public ConnectionString(string dataSourceValue) => this.dataSourceValue = dataSourceValue;

    public static implicit operator string(ConnectionString connectionString) => connectionString.Formatted == null ?
                                                                                    $"{dataSource}={connectionString.dataSourceValue}" :
                                                                                    $"{dataSource}={connectionString.dataSourceValue}; Formatted={connectionString.Formatted.Value}";
    public static implicit operator ConnectionString(string dataSourceValue) => new ConnectionString(dataSourceValue);

    public bool? Formatted { get; set; }

    public ConnectionString AddFormatted(bool formatted) => new(dataSourceValue) { Formatted = formatted };

    public ConnectionString Sandbox(string sandboxRootPath, string sandboxId)
    {
        //Make sure that the sandbox root folder exists.
        if (!Directory.Exists(sandboxRootPath))
            Directory.CreateDirectory(sandboxRootPath);

        var fullSandboxPath = Path.Combine(sandboxRootPath, sandboxId);
        var folderAsDatabase = Directory.Exists(dataSourceValue);

        //If the sandbox folder exists then clean it out.
        var sandboxExists = Directory.Exists(fullSandboxPath);
        if (sandboxExists)
            Directory.Delete(fullSandboxPath, true);

        Directory.CreateDirectory(fullSandboxPath);

        if (folderAsDatabase)
        {

            //Copy the existing folder database to the new location

            //Copy all the files & replace any files with the same name
            foreach (string sourcePath in Directory.GetFiles(dataSourceValue, "*.*", SearchOption.TopDirectoryOnly))
            {
                //Get the file name.
                var fileName = Path.GetFileName(sourcePath);

                File.Copy(sourcePath, Path.Combine(fullSandboxPath, fileName), true);
            }

            return new(fullSandboxPath) { Formatted = Formatted };
        }

        //This is a file as database option.  

        //Get the file name.
        var databaseFileName = Path.GetFileName(dataSourceValue);

        var sandboxDatabaseFile = Path.Combine(fullSandboxPath, databaseFileName);
        File.Copy(dataSourceValue, sandboxDatabaseFile, true);

        return new(sandboxDatabaseFile) { Formatted = Formatted };
    }
}
