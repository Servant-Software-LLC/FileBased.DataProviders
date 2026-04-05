using System.Text;
using Xunit;

namespace Data.Csv.Tests;

public class CsvTransformStreamTests
{
    [Fact]
    public void Read_ReturnsHeaderAndDataWithConsistentColumns()
    {
        // Arrange:
        // Header: "A,B,C"
        // Second row missing one column: "1,2"
        // Third row: "3,4,5"
        string csvContent = "A,B,C\n1,2\n3,4,5\n";
        string expectedOutput = "A,B,C\n1,2,\n3,4,5\n";

        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
        using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
        {
            // Create CsvTransformStream with comma as separator.
            using (var csvStream = new CsvTransformStream(sr, ','))
            {
                // Read all bytes from the CsvTransformStream.
                using (MemoryStream output = new MemoryStream())
                {
                    byte[] buffer = new byte[16];
                    int n;
                    while ((n = csvStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, n);
                    }
                    string result = Encoding.UTF8.GetString(output.ToArray());
                    Assert.Equal(expectedOutput, result);
                }
            }
        }
    }

    [Fact]
    public void Read_PreservesOverflowBetweenReads()
    {
        // Arrange:
        // Create CSV content that will cause multiple Read calls with partial fills.
        // The header line is "Col1,Col2,Col3", second row "a,b,c", third row "d,e,f".
        string csvContent = "Col1,Col2,Col3\na,b,c\nd,e,f\n";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
        using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
        {
            // Use a small buffer size to force overflow.
            using (var csvStream = new CsvTransformStream(sr, ','))
            {
                // Read repeatedly with a buffer smaller than the total CSV text.
                byte[] smallBuffer = new byte[10];
                MemoryStream output = new MemoryStream();
                int bytesRead;
                while ((bytesRead = csvStream.Read(smallBuffer, 0, smallBuffer.Length)) > 0)
                {
                    string smallString = Encoding.UTF8.GetString(smallBuffer, 0, bytesRead);
                    output.Write(smallBuffer, 0, bytesRead);
                }
                string result = Encoding.UTF8.GetString(output.ToArray());
                Assert.Equal(csvContent, result);
            }
        }
    }

    [Fact]
    public void Seek_ResetsStreamCorrectly()
    {
        // Arrange:
        // Create a simple CSV file.
        string csvContent = "X,Y,Z\n10,20,30\n40,50,60\n";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
        using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
        {
            using (var csvStream = new CsvTransformStream(sr, ','))
            {
                // Read some data.
                byte[] buffer = new byte[20];
                int firstRead = csvStream.Read(buffer, 0, buffer.Length);
                Assert.True(firstRead > 0);

                // Now call Seek(0, Begin).
                long pos = csvStream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(0, pos);

                // Read again.
                byte[] buffer2 = new byte[100];
                int secondRead = csvStream.Read(buffer2, 0, buffer2.Length);
                string result = Encoding.UTF8.GetString(buffer2, 0, secondRead);

                // Expect the full CSV content.
                Assert.Equal(csvContent, result);
            }
        }
    }

}
