namespace Data.Tests.Common.Utils;

/// <summary>
/// A helper stream that wraps an inner stream but always reports CanSeek as false.
/// This simulates a forward-only stream.
/// </summary>
public class NonSeekableStream : Stream
{
    private readonly Stream inner;

    public NonSeekableStream(Stream inner) => this.inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => inner.CanWrite;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() => inner.Flush();
    public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => inner.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);
}
