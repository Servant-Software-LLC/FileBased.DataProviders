using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Data.Common.Extension;

public static class FileConnectionStringTestsExtensions
{
    public const string SourcesFolder = "Sources";
    public static string SourcesPristineCopy => $"Sources.Pristine_{RuntimeInformation.FrameworkDescription}";

    public static FileConnectionString Sandbox(this FileConnectionString fileConnectionString, string sandboxRootPath, string sandboxId)
    {
        //Make sure that the sandbox root folder exists.
        if (!Directory.Exists(sandboxRootPath))
            Directory.CreateDirectory(sandboxRootPath);

        var fullSandboxPath = Path.Combine(sandboxRootPath, sandboxId);

        //TODO: This approach isn't 'clean', in that it assumes that the Sources folder is the base of each Data Source.  Anyhow
        //      for now, this will work because this is all in only the Unit Test projects.
        var dataSourceValue = fileConnectionString.DataSource;
        if (!dataSourceValue.StartsWith(SourcesFolder))
            throw new Exception($"Expected the data source value to begin with a path in the {SourcesFolder} folder.  dataSourceValue: {dataSourceValue}");
        
        string runningFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string pristineDataSourceValue = Path.Combine(runningFolder, SourcesPristineCopy + dataSourceValue.Substring(SourcesFolder.Length));


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

            return new() { DataSource = fullSandboxPath, Formatted = fileConnectionString.Formatted };
        }

        //This is a file as database option.  

        //Get the file name.
        var databaseFileName = Path.GetFileName(pristineDataSourceValue);

        var sandboxDatabaseFile = Path.Combine(fullSandboxPath, databaseFileName);
        File.Copy(pristineDataSourceValue, sandboxDatabaseFile, true);

        return new() { DataSource = sandboxDatabaseFile, Formatted = fileConnectionString.Formatted };
    }

    public static FileConnectionString AddLogging(this FileConnectionString fileConnectionString, LogLevel minimumLogLevel)
    {
        var clone = fileConnectionString.Clone();
        clone.LogLevel = minimumLogLevel;
        return clone;
    }    
}
