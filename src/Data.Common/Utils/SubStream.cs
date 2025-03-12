namespace Data.Common.Utils;

/// <summary>
/// A stream that reads from a specified region of an underlying stream.
/// </summary>
public class SubStream : Stream
{
    private readonly Stream baseStream;
    private readonly long start;
    private readonly long length;
    private long position;

    public SubStream(Stream baseStream, long start, long length)
    {
        this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        this.start = start;
        this.length = length;
        position = 0;
    }

    public override bool CanRead => baseStream.CanRead;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => length;

    public override long Position
    {
        get => position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (position >= length)
            return 0;
        // Position the base stream at the appropriate offset.
        baseStream.Seek(start + position, SeekOrigin.Begin);
        int toRead = (int)Math.Min(count, length - position);
        int read = baseStream.Read(buffer, offset, toRead);
        position += read;
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPos;
        switch (origin)
        {
            case SeekOrigin.Begin:
                newPos = offset;
                break;
            case SeekOrigin.Current:
                newPos = position + offset;
                break;
            case SeekOrigin.End:
                newPos = length + offset;
                break;
            default:
                throw new ArgumentException("Invalid seek origin", nameof(origin));
        }
        if (newPos < 0 || newPos > length)
            throw new IOException("Attempt to seek outside the substream bounds.");
        position = newPos;
        return position;
    }

    public override void SetLength(long value) =>
        throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();
}
