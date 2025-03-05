namespace Data.Common.Utils;

/// <summary>
/// A stream wrapper that ignores seek calls.
/// This is useful when you have a forward-only stream that you want to use for
/// paged reading. The first call to DataFrame.LoadCsv is used to determine the schema,
/// and subsequent calls are supplied with the schema, so they do not require re-reading
/// the header from the beginning of the stream.
/// </summary>
public class NonResettingStreamWrapper : Stream
{
    private readonly Stream innerStream;

    public NonResettingStreamWrapper(Stream innerStream)
    {
        this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    public override bool CanRead => innerStream.CanRead;
    public override bool CanSeek => true; // We claim to support seeking, but...
    public override bool CanWrite => innerStream.CanWrite;
    public override long Length => innerStream.Length;

    /// <summary>
    /// Returns the current position of the inner stream.
    /// </summary>
    public override long Position
    {
        get => innerStream.Position;
        set { /* Ignore setting the position to keep the current position */ }
    }

    public override void Flush() => innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) =>
        innerStream.Read(buffer, offset, count);

    /// <summary>
    /// Ignores all Seek calls and simply returns the current position.
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
    {
        // Ignore any seek operation: just return the current position.
        return innerStream.Position;
    }

    public override void SetLength(long value) => innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) =>
        innerStream.Write(buffer, offset, count);
}
