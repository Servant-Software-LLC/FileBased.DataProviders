namespace Data.Common.Utils;

/// <summary>
/// A stream that reads from a first stream until it's exhausted, then continues reading from a second stream.
/// Disposing the ConcatStream disposes both inner streams.
/// </summary>
public class ConcatStream : Stream, IDisposable
{
    private readonly Stream first;
    private readonly Stream second;
    private readonly long secondInitialPosition;
    private bool firstExhausted = false;
    private bool disposed = false;

    public ConcatStream(Stream first, Stream second)
    {
        this.first = first ?? throw new ArgumentNullException(nameof(first));
        this.second = second ?? throw new ArgumentNullException(nameof(second));

        if (!first.CanRead || !second.CanRead)
            throw new ArgumentException("Both streams must be readable.");
            
        // Save the initial position of the second stream
        this.secondInitialPosition = second.CanSeek ? second.Position : 0;
    }

    public override bool CanRead => first.CanRead && second.CanRead;
    public override bool CanSeek => first.CanSeek && second.CanSeek;
    public override bool CanWrite => false;

    public override long Length
    {
        get
        {
            if (!CanSeek)
                throw new NotSupportedException("Cannot get length of non-seekable streams.");
            return first.Length + (second.Length - secondInitialPosition);
        }
    }

    public override long Position
    {
        get
        {
            if (!CanSeek)
                throw new NotSupportedException("Cannot get position of non-seekable streams.");
            return firstExhausted ? first.Length + (second.Position - secondInitialPosition) : first.Position;
        }
        set
        {
            Seek(value, SeekOrigin.Begin);
        }
    }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!firstExhausted)
        {
            int n = first.Read(buffer, offset, count);
            if (n < count)
            {
                firstExhausted = true;
                
                // Position the second stream at its initial position before we start reading from it
                if (second.CanSeek && second.Position != secondInitialPosition)
                    second.Position = secondInitialPosition;
                    
                int m = second.Read(buffer, offset + n, count - n);
                return n + m;
            }
            return n;
        }
        return second.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (!CanSeek)
            throw new NotSupportedException("Seeking is not supported.");

        long targetPosition;
        switch (origin)
        {
            case SeekOrigin.Begin:
                targetPosition = offset;
                break;
            case SeekOrigin.Current:
                targetPosition = Position + offset;
                break;
            case SeekOrigin.End:
                targetPosition = Length + offset;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), "Invalid seek origin.");
        }

        if (targetPosition < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Seek position is out of bounds.");

        if (targetPosition < first.Length)
        {
            first.Position = targetPosition;
            firstExhausted = false;
        }
        else
        {
            // When seeking into the second stream, adjust by its initial position
            second.Position = secondInitialPosition + (targetPosition - first.Length);
            firstExhausted = true;
        }

        return Position;
    }

    public override void SetLength(long value) =>
        throw new NotSupportedException("Setting length is not supported.");

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("Writing is not supported.");

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
}
