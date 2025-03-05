namespace Data.Tests.Common.Utils;

public abstract class UnendingStream : Stream
{
    private readonly Queue<byte> _buffer = new Queue<byte>();
    private bool _headOfStream = true;

    protected abstract byte[] GetMoreBytes();

    /// <summary>
    /// When a read is requested, the stream will call the producer until at least <paramref name="count"/> bytes
    /// are available (or the producer returns no data).
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0 || count < 0 || (offset + count) > buffer.Length)
            throw new ArgumentOutOfRangeException();

        _headOfStream = false;

        // Fill the internal buffer until we have at least "count" bytes.
        while (_buffer.Count < count)
        {
            byte[] produced = GetMoreBytes();
            if (produced == null || produced.Length == 0)
            {
                // No more data is coming; break out and return what we have.
                break;
            }
            foreach (byte b in produced)
            {
                _buffer.Enqueue(b);
            }
        }

        // Dequeue up to "count" bytes.
        int bytesRead = 0;
        while (_buffer.Count > 0 && bytesRead < count)
        {
            buffer[offset + bytesRead] = _buffer.Dequeue();
            bytesRead++;
        }
        return bytesRead;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush() { /* No-op */ }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (_headOfStream && origin != SeekOrigin.Begin && offset != 0)
            throw new NotSupportedException();

        return 0;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

}
