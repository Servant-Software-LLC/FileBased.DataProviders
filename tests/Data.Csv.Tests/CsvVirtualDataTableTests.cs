using System.IO;
using System.Linq;
using System.Text;
using Data.Common.Utils;
using Data.Common.Utils.ConnectionString;
using Data.Csv.Utils;
using Xunit;

namespace Data.Csv.Tests;

public class CsvVirtualDataTableTests
{
    [Fact]
    public void GuardShapedValue_WhenGuardStrippingDisabled_IsReturnedUnchanged()
    {
        // A value that merely looks like a whitespace guard (sentinel + whitespace + sentinel) must NOT
        // be altered by a reader whose source does not use guards. Guard stripping is opt-in via
        // stripWhitespaceGuard (used only by the XLS provider, which writes the guards); every other
        // provider must leave such data exactly as-is.
        var guard = CsvWhitespaceGuard.Sentinel;
        var guardShaped = $"{guard}  {guard}";
        var csv = $"Value,Marker\n{guardShaped},guarded\nnormal,control\n";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        using var reader = new StreamReader(stream);
        using var table = new CsvVirtualDataTable(
            reader, "guardShaped", pageSize: 4096, guessTypeRows: 1000,
            FloatingPointDataType.Double, TypeGuesser.GuessType, separator: ',',
            stripWhitespaceGuard: false);

        var rows = table.Rows!.ToList();
        var guardedRow = rows.Single(row => (string)row["Marker"] == "guarded");

        Assert.Equal(guardShaped, (string)guardedRow["Value"]);
    }
}
