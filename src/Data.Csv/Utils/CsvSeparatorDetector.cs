using CsvHelper.Configuration;
using CsvHelper.Delegates;
using Data.Common.Utils;
using System.Globalization;

namespace Data.Csv.Utils;

public class CsvSeparatorDetector
{
    private readonly Stream stream;
    private readonly string[] supportedSeparators;
    private readonly int linesCount;

    public CsvSeparatorDetector(Stream stream, string[] supportedSeparators, int linesCount)
    {
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        this.supportedSeparators = supportedSeparators ?? throw new ArgumentNullException(nameof(supportedSeparators));
        this.linesCount = linesCount > 0 ? linesCount : throw new ArgumentOutOfRangeException(nameof(linesCount));

        this.stream.Seek(0, SeekOrigin.Begin);
    }

    public char Detect()
    {
        SubStreamReader subStreamReader = new(stream, linesCount);
        var sample = subStreamReader.ReadToLimit();

        // The Culture ListSeparator is set as "*DoNotDetect" because the CsvHelper checks it when determining the separator used.
        // If the ListSeparator is present in every CSV line analyzed for separator detection, it will be used as the separator without considering the ranking of all the separators in the CSV file.
        // For CultureInfo.InvariantCulture, the ListSeparator will always be a comma. So, if a comma is found in every line even if it's not the real separator, it will be returned as separator of the GetDelimiter method.
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.TextInfo.ListSeparator = "*DoNotDetect";
        var config = new CsvConfiguration(culture)
        {
            DetectDelimiterValues = supportedSeparators
        };

        var detectedDelimiter =
            ConfigurationFunctions.GetDelimiter(new GetDelimiterArgs(sample, config));

        return detectedDelimiter switch
        {
            "," => ',',
            ";" => ';',
            "|" => '|',
            "\t" => '\t',
            _ => ','
        };
    }
}
