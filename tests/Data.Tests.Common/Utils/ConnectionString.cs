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

        //TODO: This approach isn't 'clean', in that it assumes that the Sources folder is the base of each Data Source.  Anyhow
        //      for now, this will work because this is all in only the Unit Test projects.
        if (!dataSourceValue.StartsWith(ConnectionStringsBase.SourcesFolder))
            throw new Exception($"Expected the data source value to begin with a path in the {ConnectionStringsBase.SourcesFolder} folder.  dataSourceValue: {dataSourceValue}");
        string pristineDataSourceValue = ConnectionStringsBase.SourcesPristineCopy + dataSourceValue.Substring(ConnectionStringsBase.SourcesFolder.Length);


        var folderAsDatabase = Directory.Exists(pristineDataSourceValue);

        //If the sandbox folder exists then clean it out.
        var sandboxExists = Directory.Exists(fullSandboxPath);
        if (sandboxExists)
            Directory.Delete(fullSandboxPath, true);

        Directory.CreateDirectory(fullSandboxPath);

        if (folderAsDatabase)
        {

            //Copy the existing folder database to the new location

            //Copy all the files & replace any files with the same name
            foreach (string sourcePath in Directory.GetFiles(pristineDataSourceValue, "*.*", SearchOption.TopDirectoryOnly))
            {
                //Get the file name.
                var fileName = Path.GetFileName(sourcePath);

                File.Copy(sourcePath, Path.Combine(fullSandboxPath, fileName), true);
            }

            return new(fullSandboxPath) { Formatted = Formatted };
        }

        //This is a file as database option.  

        //Get the file name.
        var databaseFileName = Path.GetFileName(pristineDataSourceValue);

        var sandboxDatabaseFile = Path.Combine(fullSandboxPath, databaseFileName);
        File.Copy(pristineDataSourceValue, sandboxDatabaseFile, true);

        return new(sandboxDatabaseFile) { Formatted = Formatted };
    }
}
