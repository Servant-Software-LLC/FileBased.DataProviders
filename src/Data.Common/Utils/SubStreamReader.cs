namespace Data.Common.Utils;

public class SubStreamReader
{
    private readonly Stream stream;
    private readonly int linesLimit;
    private readonly StreamReader streamReader;

    public SubStreamReader(Stream stream, int linesLimit)
    {
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        this.linesLimit = linesLimit > 0 ? linesLimit : throw new ArgumentOutOfRangeException(nameof(linesLimit), "Must be greater than zero.");
        streamReader = new StreamReader(stream, null, true, -1, leaveOpen: true);
    }

    public string ReadToLimit()
    {
        var lines = string.Join("\n", ReadLines());
        stream.Seek(0, SeekOrigin.Begin);
        return lines;
    }

    private IEnumerable<string> ReadLines()
    {
        for (var i = 0; i < linesLimit; i++)
        {
            var line = streamReader.ReadLine();
            if (line is null)
            {
                yield break;
            }

            if (!string.IsNullOrEmpty(line))
            {
                yield return line;
            }
        }
    }

}
