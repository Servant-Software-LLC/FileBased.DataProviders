using System.Text;
using Data.Common.Utils;
using Xunit;

namespace Data.Tests.Common.Utils
{
    public class ConcatStreamTests
    {
        [Fact]
        public void ConcatStream_JsonVirtualDataTableScenario_ShouldSkipPreambleData()
        {
            // Arrange
            // Simplified version of what happens in JsonVirtualDataTable

            // This represents the original content stream that will be read from
            var preamble = "Preamble.";
            var remainingDataInStream = "Remaining data";
            var content = $"{preamble}{remainingDataInStream}";
            var originalStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            
            // Read first part (preamble) to determine schema
            var preambleBuffer = new byte[preamble.Length]; // Read first 9 bytes as preamble
            originalStream.Read(preambleBuffer, 0, preambleBuffer.Length);
            var preambleStream = new MemoryStream(preambleBuffer);
            
            // Now originalStream's position is at 9, not at the beginning!
            Assert.Equal(preamble.Length, originalStream.Position);
            
            // Act - Create a ConcatStream like JsonVirtualDataTable does
            var concatStream = new ConcatStream(preambleStream, originalStream);
            var offsetIntoSecondStream = 1;
            concatStream.Position = preambleStream.Length + offsetIntoSecondStream;
            
            var result = ReadAllRemainingText(concatStream);

            // Assert
            // Should get all but the first character of the remaining stream's position
            Assert.Equal(remainingDataInStream.Substring(offsetIntoSecondStream), result);
        }
        
        private string ReadAllRemainingText(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            {
                return reader.ReadToEnd();
            }
        }
    }
} 