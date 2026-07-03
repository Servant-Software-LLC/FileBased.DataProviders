namespace Data.Common.Utils;

/// <summary>
/// Protocol constant shared between a CSV-producing provider (e.g. the XLS provider, which converts a
/// sheet to CSV) and the CSV reader.
///
/// CSV parsing goes through <c>DataFrame.LoadCsv</c>, which is backed by
/// <c>Microsoft.VisualBasic.FileIO.TextFieldParser</c> whose <c>TrimWhiteSpace</c> is <c>true</c> and is
/// not configurable. That trims a whitespace-only cell down to an empty string. A provider that must
/// preserve such whitespace wraps the value in this guard on both ends (so the field's edges are
/// non-whitespace and survive parsing); the reader strips the guard afterwards. A Unicode private-use
/// character is used so it will not collide with real content.
/// </summary>
public static class CsvWhitespaceGuard
{
    public const char Sentinel = (char)0xE000;
}
