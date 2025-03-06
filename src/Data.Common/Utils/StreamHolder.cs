namespace Data.Common.Utils;

class StreamHolder
{
    public Func<Stream> StreamCreationFunc { get; set; }
    public BufferedResetStream LatestReadStream { get; set; }
    public MemoryStream LatestWriteStream { get; set; }
}
