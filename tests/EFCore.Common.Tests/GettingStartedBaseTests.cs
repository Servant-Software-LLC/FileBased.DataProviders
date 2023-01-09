using EFCore.Common.Tests.Models;

namespace EFCore.Common.Tests;

public abstract class GettingStartedBaseTests<TBloggingContext> where TBloggingContext : BloggingContextBase, new()
{
    /// <summary>
    /// Creates a new connection string in a different temp folder for each test execution
    /// </summary>
    protected string CreateTempFolder()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    protected string CreateTempFile(string fileExension)
    {
        var tempFile = Path.GetTempFileName();
        var tempFileWithExtension = $"{tempFile}.{fileExension}";
        File.Move(tempFile, tempFileWithExtension);
        return tempFileWithExtension;
    }

    protected void Logic_Create(string connectionString) 
    {
        using var db = new TBloggingContext();
        db.ConnectionString = connectionString;

        db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();
    }
}
