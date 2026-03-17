using System;
using System.IO;
using System.Text;
using Data.Json.Utils;
using Xunit;

namespace Data.Json.Tests.Utils
{
    public class BOMHandlingStreamTests
    {
        private static readonly byte[] Utf8BOM = new byte[] { 0xEF, 0xBB, 0xBF };

        [Fact]
        public void Constructor_WithNullStream_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BOMHandlingStream(null));
        }

        [Fact]
        public void Read_StreamWithBOM_SkipsBOM()
        {
            // Arrange
            var content = "{}";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer = new byte[100];

            // Act
            int bytesRead = bomStream.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.Equal(content.Length, bytesRead);
            Assert.Equal(Encoding.UTF8.GetBytes(content), buffer.AsSpan(0, bytesRead).ToArray());
        }

        [Fact]
        public void Read_StreamWithoutBOM_ReadsNormally()
        {
            // Arrange
            var content = "{}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var bomStream = new BOMHandlingStream(stream);
            var buffer = new byte[100];

            // Act
            int bytesRead = bomStream.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.Equal(content.Length, bytesRead);
            Assert.Equal(Encoding.UTF8.GetBytes(content), buffer.AsSpan(0, bytesRead).ToArray());
        }

        [Fact]
        public void Seek_StreamWithBOM_AdjustsPosition()
        {
            // Arrange
            var content = "0123456789";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer = new byte[5];

            // Act - Read some bytes then seek back to beginning
            int initialBytesRead = bomStream.Read(buffer, 0, buffer.Length);
            bomStream.Seek(0, SeekOrigin.Begin);
            var resetBuffer = new byte[5];
            int bytesAfterReset = bomStream.Read(resetBuffer, 0, resetBuffer.Length);

            // Assert
            Assert.Equal(5, initialBytesRead);
            Assert.Equal(5, bytesAfterReset);
            Assert.Equal(buffer, resetBuffer); // Should read the same content both times
        }

        [Fact]
        public void ReadMultiple_StreamWithBOM_ReadsProperly()
        {
            // Arrange
            var content = "0123456789";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer1 = new byte[4];
            var buffer2 = new byte[4];
            var buffer3 = new byte[4];

            // Act - Read in multiple chunks
            int bytesRead1 = bomStream.Read(buffer1, 0, buffer1.Length);
            int bytesRead2 = bomStream.Read(buffer2, 0, buffer2.Length);
            int bytesRead3 = bomStream.Read(buffer3, 0, buffer3.Length);

            // Assert
            Assert.Equal(4, bytesRead1);
            Assert.Equal(4, bytesRead2);
            Assert.Equal(2, bytesRead3); // Last chunk should have only 2 bytes
            
            Assert.Equal("0123", Encoding.UTF8.GetString(buffer1, 0, bytesRead1));
            Assert.Equal("4567", Encoding.UTF8.GetString(buffer2, 0, bytesRead2));
            Assert.Equal("89", Encoding.UTF8.GetString(buffer3, 0, bytesRead3));
        }

        [Fact]
        public void Position_Get_ReturnsLogicalPosition()
        {
            // Arrange
            var content = "0123456789";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer = new byte[5];

            // Act
            bomStream.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.Equal(5, bomStream.Position);
        }

        [Fact]
        public void Position_Set_SeeksToLogicalPosition()
        {
            // Arrange
            var content = "0123456789";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer = new byte[5];

            // Act
            bomStream.Position = 3;
            int bytesRead = bomStream.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.Equal(5, bytesRead);
            Assert.Equal("34567", Encoding.UTF8.GetString(buffer, 0, bytesRead));
            Assert.Equal(8, bomStream.Position);
        }

        [Fact]
        public void ReadOffsetLength_StreamWithBOM_SkipsBOMAndReadsCorrectly()
        {
            // Arrange
            var content = "0123456789";
            var streamWithBom = CreateStreamWithBOM(content);
            var bomStream = new BOMHandlingStream(streamWithBom);
            var buffer = new byte[10];

            // Act - Read with offset and count
            int bytesRead = bomStream.Read(buffer, 2, 5);

            // Assert
            Assert.Equal(5, bytesRead);
            Assert.Equal(new byte[] { 0, 0, (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', 0, 0, 0 }, buffer);
        }

        [Fact]
        public void PartialBOM_StreamWithPartialBOM_HandlesCorrectly()
        {
            // Arrange - Create a stream with only part of a BOM
            var partialBomStream = new MemoryStream();
            partialBomStream.Write(new byte[] { 0xEF, 0xBB }, 0, 2); // Only 2 bytes of the BOM
            partialBomStream.Write(Encoding.UTF8.GetBytes("test"), 0, 4);
            partialBomStream.Seek(0, SeekOrigin.Begin);
            
            var bomStream = new BOMHandlingStream(partialBomStream);
            var buffer = new byte[6];

            // Act
            int bytesRead = bomStream.Read(buffer, 0, buffer.Length);

            // Assert - Should read the partial BOM as part of the content
            Assert.Equal(6, bytesRead);
            Assert.Equal(new byte[] { 0xEF, 0xBB, (byte)'t', (byte)'e', (byte)'s', (byte)'t' }, buffer.AsSpan(0, bytesRead).ToArray());
        }

        [Fact]
        public void Dispose_DisposesUnderlyingStream()
        {
            // Arrange
            var ms = new MemoryStream();
            var bomStream = new BOMHandlingStream(ms);

            // Act
            bomStream.Dispose();

            // Assert - This should throw because the underlying stream is closed
            Assert.Throws<ObjectDisposedException>(() => ms.Position = 0);
        }

        // Helper method to create a stream with a BOM
        private static MemoryStream CreateStreamWithBOM(string content)
        {
            var stream = new MemoryStream();
            stream.Write(Utf8BOM, 0, Utf8BOM.Length);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            stream.Write(contentBytes, 0, contentBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
} 