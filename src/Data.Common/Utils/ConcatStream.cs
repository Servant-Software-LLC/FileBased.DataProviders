namespace Data.Common.Utils;

/// <summary>
/// A simple stream that reads from a first stream until it’s exhausted, then continues reading from a second stream.
/// Disposing the ConcatStream disposes both inner streams.
/// </summary>
public class ConcatStream : Stream, IDisposable
{
    private readonly Stream first;
    private readonly Stream second;
    private bool firstExhausted = false;
    private bool disposed = false;

    public ConcatStream(Stream first, Stream second)
    {
        this.first = first ?? throw new ArgumentNullException(nameof(first));
        this.second = second ?? throw new ArgumentNullException(nameof(second));
    }

    public override bool CanRead => first.CanRead && second.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!firstExhausted)
        {
            int n = first.Read(buffer, offset, count);
            if (n < count)
            {
                firstExhausted = true;
                int m = second.Read(buffer, offset + n, count - n);
                return n + m;
            }
            return n;
        }
        return second.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
    public override void SetLength(long value) =>
        throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                first.Dispose();
                second.Dispose();
            }
            disposed = true;
        }
        base.Dispose(disposing);
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
