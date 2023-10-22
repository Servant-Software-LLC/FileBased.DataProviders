using Data.Common.Extension;
using EFCore.Common.Tests.Utils;
using System.Runtime.CompilerServices;

namespace EFCore.Json.Tests;

internal static class AssemblyInitializer
{
    //Before any of the unit tests run, we need to keep a copy of the databases Sources for our sandboxing
    //to use.  The reason for this, is that the sandboxing doesn't have a way to respect
    //the FileWriter._rwLock (ReaderWriterLockSlim) and so, if a sandboxing area is created while other 
    //unit tests are in progress, there is a chance that it grabs a corrupt version of the files on disk.
    [ModuleInitializer]
    public static void Initialize()
    {
        FileUtils.CopyDirectory(FileConnectionStringTestsExtensions.SourcesFolder, FileConnectionStringTestsExtensions.SourcesPristineCopy, true);
    }
}
