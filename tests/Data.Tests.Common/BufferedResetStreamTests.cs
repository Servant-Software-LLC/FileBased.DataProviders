using System.Reflection;
using System.Text;
using Xunit;
using Data.Common.Utils;
using Data.Tests.Common.Utils;

namespace Data.Tests.Common;

public class BufferedResetStreamTests
{
    private static byte[] ToBytes(string s) => Encoding.UTF8.GetBytes(s);
    private static string FromBytes(byte[] bytes, int count) => Encoding.UTF8.GetString(bytes, 0, count);

    [Fact]
    public void ReadWithoutReset_ReturnsOriginalData()
    {
        // Arrange: Create a known string and wrap it in a non-seekable stream.
        string original = "Hello, world! This is a test of the BufferedResetStream.";
        byte[] originalBytes = ToBytes(original);
        using var ms = new MemoryStream(originalBytes);
        using var nonSeekable = new NonSeekableStream(ms);
        // Use allowedResets = 1 (default behavior)
        using var bufferedStream = new BufferedResetStream(nonSeekable, allowedResets: 1);

        byte[] buffer = new byte[originalBytes.Length];
        int totalRead = 0;
        int bytesRead;

        // Act: Read the entire stream without resetting.
        while ((bytesRead = bufferedStream.Read(buffer, totalRead, buffer.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }

        string result = FromBytes(buffer, totalRead);

        // Assert: The read data should equal the original.
        Assert.Equal(original, result);
    }

    [Fact]
    public void SingleReset_ReplaysBufferedData()
    {
        // Arrange: Create a known string.
        string original = "0123456789ABCDEFGHIJ"; // 20 characters
        byte[] originalBytes = ToBytes(original);
        using var ms = new MemoryStream(originalBytes);
        using var nonSeekable = new NonSeekableStream(ms);
        // Allow one reset.
        using var bufferedStream = new BufferedResetStream(nonSeekable, allowedResets: 1);

        // Act: Read a partial amount (10 bytes) first.
        byte[] partialBuffer = new byte[10];
        int bytesRead = bufferedStream.Read(partialBuffer, 0, partialBuffer.Length);
        string partialResult = FromBytes(partialBuffer, bytesRead);
        Assert.Equal("0123456789", partialResult);

        // Now perform the reset.
        bufferedStream.Seek(0, SeekOrigin.Begin);

        // Read the entire stream from the reset position.
        byte[] allBuffer = new byte[originalBytes.Length];
        int totalRead = 0;
        while ((bytesRead = bufferedStream.Read(allBuffer, totalRead, allBuffer.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }
        string resultAfterReset = FromBytes(allBuffer, totalRead);

        // Assert: The re-read data should equal the original string.
        Assert.Equal(original, resultAfterReset);
    }

    [Fact]
    public void MultipleResets_ReplaysBufferedDataMultipleTimes()
    {
        // Arrange: Create a known string.
        string original = "abcdefghijklmnopqrstuvwxyz"; // 26 characters
        byte[] originalBytes = ToBytes(original);
        using var ms = new MemoryStream(originalBytes);
        using var nonSeekable = new NonSeekableStream(ms);
        // Allow two resets.
        using var bufferedStream = new BufferedResetStream(nonSeekable, allowedResets: 2);

        // Read partial data first.
        byte[] buffer1 = new byte[10];
        int bytesRead = bufferedStream.Read(buffer1, 0, buffer1.Length);
        string part1 = FromBytes(buffer1, bytesRead);
        Assert.Equal("abcdefghij", part1);

        // First reset.
        bufferedStream.Seek(0, SeekOrigin.Begin);

        // Read entire stream after first reset.
        byte[] buffer2 = new byte[originalBytes.Length];
        int totalRead = 0;
        while ((bytesRead = bufferedStream.Read(buffer2, totalRead, buffer2.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }
        string resultAfterFirstReset = FromBytes(buffer2, totalRead);
        Assert.Equal(original, resultAfterFirstReset);

        // Second reset.
        bufferedStream.Seek(0, SeekOrigin.Begin);

        // Read entire stream after second reset.
        byte[] buffer3 = new byte[originalBytes.Length];
        totalRead = 0;
        while ((bytesRead = bufferedStream.Read(buffer3, totalRead, buffer3.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }
        string resultAfterSecondReset = FromBytes(buffer3, totalRead);
        Assert.Equal(original, resultAfterSecondReset);

        // Further reset should throw.
        Assert.Throws<NotSupportedException>(() => bufferedStream.Seek(0, SeekOrigin.Begin));
    }

    [Fact]
    public void BufferIsReleasedAfterFinalResetAndBufferedDataConsumed()
    {
        // Arrange: Create a known string.
        string original = "The quick brown fox jumps over the lazy dog.";
        byte[] originalBytes = ToBytes(original);
        using var ms = new MemoryStream(originalBytes);
        using var nonSeekable = new NonSeekableStream(ms);
        // Allow one reset.
        using var bufferedStream = new BufferedResetStream(nonSeekable, allowedResets: 1);

        // Read the entire stream to buffer all data.
        byte[] buffer = new byte[originalBytes.Length];
        int totalRead = 0;
        int bytesRead;
        while ((bytesRead = bufferedStream.Read(buffer, totalRead, buffer.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }
        Assert.Equal(original, FromBytes(buffer, totalRead));

        // Perform the final (and only) reset.
        bufferedStream.Seek(0, SeekOrigin.Begin);

        // Read all buffered data from the reset stream.
        byte[] resetBuffer = new byte[originalBytes.Length];
        int totalResetRead = 0;
        while ((bytesRead = bufferedStream.Read(resetBuffer, totalResetRead, resetBuffer.Length - totalResetRead)) > 0)
        {
            totalResetRead += bytesRead;
        }
        Assert.Equal(original, FromBytes(resetBuffer, totalResetRead));

        // At this point, since no resets remain and the buffered data has been fully consumed,
        // the internal buffer should have been released.
        bool bufferReleased = bufferedStream.BufferReleased;
        Assert.True(bufferReleased, "Buffer should be released after final reset and consuming buffered data.");

        // Further reads should work (reading directly from inner stream, without buffering),
        // but since the underlying stream is already at its end, reading should return 0.
        byte[] extraBuffer = new byte[10];
        int extraRead = bufferedStream.Read(extraBuffer, 0, extraBuffer.Length);
        Assert.Equal(0, extraRead);
    }
}
