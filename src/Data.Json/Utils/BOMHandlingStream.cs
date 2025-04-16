using System.Diagnostics.Metrics;

namespace Data.Json.Utils;

/// <summary>
/// A stream wrapper that detects and removes Byte Order Marks (BOMs) when reading from the underlying stream.
/// </summary>
public class BOMHandlingStream : Stream
{
    // UTF-8 BOM bytes: EF BB BF
    private static readonly byte[] Utf8BOM = new byte[] { 0xEF, 0xBB, 0xBF };
    
    private readonly Stream baseStream;
    private byte[] nonBomBytes = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="BOMHandlingStream"/> class.
    /// </summary>
    /// <param name="baseStream">The underlying stream to read from.</param>
    public BOMHandlingStream(Stream baseStream)
    {
        this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));

        if (baseStream.Position != 0)
        {
            throw new InvalidOperationException("The base stream must be at its initial position in order to check for BOM on first Read");
        }

        if (!baseStream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", nameof(baseStream));
        }
    }

    private void CheckForBom()
    {
        if (CheckedBOM)
            return;

        // Check for BOM
        byte[] bom = new byte[Utf8BOM.Length];
        int bytesRead = baseStream.Read(bom, 0, bom.Length);
        
        var hasBom = false;
        // Only consider it a BOM if we read the complete UTF-8 BOM sequence (all 3 bytes)
        if (bytesRead == Utf8BOM.Length)
        {
            hasBom = true;
            for (int i = 0; i < Utf8BOM.Length; i++)
            {
                if (bom[i] != Utf8BOM[i])
                {
                    hasBom = false;
                    break;
                }
            }
        }

        // Whether we have a BOM or not, nonBomBytes != null indicates that we've checked.

        //If a BOM, then an empty MemoryStream will be saved.
        if (hasBom)
        {
            nonBomBytes = new byte[0];
            return;
        }

        // If no BOM was found or it was partial, save bytes to provide later.  
        nonBomBytes = new byte[bytesRead];
        Buffer.BlockCopy(bom, 0, nonBomBytes, 0, bytesRead);
    }

    public bool CheckedBOM => nonBomBytes != null;

    /// <inheritdoc />
    public override bool CanRead => baseStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => baseStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => baseStream.CanWrite;

    /// <inheritdoc />
    public override long Length
    {
        get
        {
            //Check for BOM, if needed.
            CheckForBom();

            return baseStream.Length - (Utf8BOM.Length - nonBomBytes.Length);
        }
    }

    /// <inheritdoc />
    public override long Position
    {
        get
        {
            CheckForBom();
            return baseStream.Position - (Utf8BOM.Length - nonBomBytes.Length);
        }
        set
        {
            CheckForBom();
            baseStream.Position = value + (Utf8BOM.Length - nonBomBytes.Length);
        }
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        // Check for BOM on first read if we haven't already
        CheckForBom();

        // If there was a BOM
        if (nonBomBytes.Length == 0) 
        {
            // Then pretend it wasn't ever then and read straight from the baseStream
            return baseStream.Read(buffer, offset, count);
        }

        if (count == 0)
            return 0;

        int copiedBytes = 0;

        // If we need to include the nonBomBytes
        if (baseStream.Position == nonBomBytes.Length && offset < nonBomBytes.Length)
        {
            // Copy the non-BOM bytes into the buffer
            for (int i = offset; i < nonBomBytes.Length; ++i)
            {
                buffer[i] = nonBomBytes[i];
                copiedBytes++;

                if (copiedBytes >= count)
                    return copiedBytes;
            }
        }

        return baseStream.Read(buffer, offset + copiedBytes, count - copiedBytes) + copiedBytes;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (!CanSeek)
            throw new NotSupportedException("This stream does not support seeking");

        // Check for BOM on first seek if we haven't already
        CheckForBom();

        // For now, let's not support seeking from anything but the begin.
        if (origin != SeekOrigin.Begin)
            throw new InvalidOperationException("Only support seeking from SeekOrigin.Begin");

        return baseStream.Seek(offset + (Utf8BOM.Length - nonBomBytes.Length), origin);
    }

    /// <inheritdoc />
    public override void Flush() => baseStream.Flush();

    /// <inheritdoc />
    public override void SetLength(long value) => throw new InvalidOperationException("Cannot set length.");

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException("Cannot write.");

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            baseStream.Dispose();
        }
        base.Dispose(disposing);
    }
} 