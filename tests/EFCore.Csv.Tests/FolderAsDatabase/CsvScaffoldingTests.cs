using EFCore.Csv.Tests.Utils;
using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Csv.Design.Internal;

namespace EFCore.Csv.Tests.FolderAsDatabase;

public class CsvScaffoldingTests
{
    [Fact]
    public void ValidateScaffolding()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        string connectionString = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new CsvDesignTimeServices());
    }

}
