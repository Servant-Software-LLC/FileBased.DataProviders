namespace Data.Common.Utils;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

/// <summary>
/// A stream wrapper that takes a forward-only stream and buffers all bytes read.
/// It supports a configurable number of resets. Each call to Seek(0, Begin) resets
/// the stream to the beginning of the buffered data (and then continues reading
/// from the inner stream as needed), until the allowed resets are exhausted.
/// Once the final allowed reset has been used and the buffered data has been fully consumed,
/// the buffer is released to free up memory.
/// </summary>
public class BufferedResetStream : Stream
{
    private readonly Stream innerStream;      // The forward-only underlying stream.
    private MemoryStream bufferStream;          // Accumulates all bytes read.
    private int remainingResets;                // Number of resets still allowed.
    private bool hasResetOccurred;              // Indicates if a reset has occurred at least once.
    private long resetReadOffset;               // Read pointer into the buffer after a reset.
    private long logicalPosition;               // Total number of bytes read from the inner stream.
    public bool BufferReleased { get; private set; }    // Indicates if the buffer has been released.  Useful for unit testing.

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedResetStream"/> class.
    /// </summary>
    /// <param name="innerStream">
    /// The underlying stream. This stream must be forward-only (i.e. its CanSeek is false).
    /// </param>
    /// <param name="allowedResets">
    /// The number of times that Seek(0, Begin) is allowed.
    /// </param>
    public BufferedResetStream(Stream innerStream, int allowedResets)
    {
        this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        if (allowedResets < 0)
            throw new ArgumentOutOfRangeException(nameof(allowedResets), "allowedResets cannot be negative.");

        bufferStream = new MemoryStream();
        remainingResets = allowedResets;
        hasResetOccurred = false;
        resetReadOffset = 0;
        logicalPosition = 0;
        BufferReleased = false;
    }

    public override bool CanRead => true;

    /// <summary>
    /// We simulate seeking (to support configurable resets).
    /// </summary>
    public override bool CanSeek => true;

    public override bool CanWrite => false;

    /// <summary>
    /// Length is available only after at least one reset, because until then
    /// we do not know the full length of the underlying stream.
    /// </summary>
    public override long Length
    {
        get
        {
            if (!hasResetOccurred)
                throw new NotSupportedException("Length is not supported until after a reset.");
            return bufferStream?.Length ?? logicalPosition;
        }
    }

    /// <summary>
    /// Position is the current read position:
    /// - Before any reset: the total bytes read from the inner stream.
    /// - After a reset: the read pointer within the internal buffer (if still available) or the logical position.
    /// </summary>
    public override long Position
    {
        get => hasResetOccurred ? resetReadOffset : logicalPosition;
        set => throw new NotSupportedException("Setting Position is not supported.");
    }

    public override void Flush()
    {
        // No flushing needed.
    }

    /// <summary>
    /// Reads data from the stream.
    /// - Before a reset has occurred, data is read directly from the inner stream and appended to the internal buffer.
    /// - After a reset, the stream first serves any buffered data (starting from the beginning) and,
    ///   if the buffer is exhausted, reads further from the inner stream (appending it to the buffer if it hasn't been released).
    ///   If no resets remain and the buffered portion has been fully consumed, the buffer is released.
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0 || count < 0 || (offset + count) > buffer.Length)
            throw new ArgumentOutOfRangeException();

        int totalRead = 0;

        if (hasResetOccurred)
        {
            // If no resets remain and the buffered data has been fully read, release the buffer.
            if (!BufferReleased && remainingResets == 0 && resetReadOffset >= bufferStream.Length)
            {
                bufferStream.Dispose();
                bufferStream = null;
                BufferReleased = true;
                //Debug.WriteLine("Buffer released.");
            }

            while (totalRead < count)
            {
                // Serve buffered data if available.
                if (!BufferReleased && bufferStream != null)
                {
                    int available = (int)(bufferStream.Length - resetReadOffset);
                    if (available > 0)
                    {
                        int toCopy = Math.Min(available, count - totalRead);
                        Array.Copy(bufferStream.GetBuffer(), resetReadOffset, buffer, offset + totalRead, toCopy);
                        resetReadOffset += toCopy;
                        totalRead += toCopy;

                        //Debug.WriteLine($"Read from buffer: {toCopy}");
                        continue;
                    }
                }

                // If buffer is released or no buffered data remains, read from the inner stream.
                int innerRead = innerStream.Read(buffer, offset + totalRead, count - totalRead);
                if (innerRead > 0)
                {
                    // Only append to the buffer if it hasn't been released.
                    if (!BufferReleased && bufferStream != null)
                    {
                        bufferStream.Write(buffer, offset + totalRead, innerRead);
                    }
                    resetReadOffset += innerRead;
                    totalRead += innerRead;
                    logicalPosition += innerRead;

                    //Debug.WriteLine($"Read from inner stream: {innerRead}");
                }
                else
                {
                    break; // End of inner stream.
                }
            }

            //Debug.WriteLine(Encoding.UTF8.GetString(buffer, offset, totalRead));
            return totalRead;
        }
        else
        {
            // Not in reset mode: read directly from inner stream and buffer the data.
            int bytesRead = innerStream.Read(buffer, offset, count);
            if (bytesRead > 0)
            {
                bufferStream.Write(buffer, offset, bytesRead);
                logicalPosition += bytesRead;
            }

            //Debug.WriteLine($"Read from inner stream & saved to buffer: {bytesRead}");
            //Debug.WriteLine(Encoding.UTF8.GetString(buffer, offset, bytesRead));
            return bytesRead;
        }
    }

    /// <summary>
    /// Supports a configurable reset.
    /// Calling Seek(0, Begin) resets the read pointer into the internal buffer to the beginning.
    /// This can be done as many times as specified by the allowed resets.
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin && offset == 0)
        {
            // If no data has been buffered yet, then there is nothing to reset.
            if (bufferStream != null && bufferStream.Length == 0)
            {
                return 0;
            }

            if (remainingResets <= 0)
                throw new NotSupportedException("No more resets allowed.");

            remainingResets--;
            hasResetOccurred = true;
            resetReadOffset = 0;

            //Debug.WriteLine("Reset performed. Remaining resets: " + remainingResets);
            return 0;
        }
        throw new NotSupportedException("Only Seek(0, Begin) is supported for resets.");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            innerStream.Dispose();
            bufferStream?.Dispose();
        }
        base.Dispose(disposing);
    }
}
