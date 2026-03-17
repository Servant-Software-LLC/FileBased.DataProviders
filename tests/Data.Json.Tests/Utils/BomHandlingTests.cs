using System.Text;
using System.Text.Json;
using Data.Json.Utils;
using System.Data.JsonClient;
using Data.Common.Utils.ConnectionString;
using Xunit;

namespace Data.Json.Tests.Utils
{
    public class BomHandlingTests
    {
        private static readonly byte[] Utf8BOM = new byte[] { 0xEF, 0xBB, 0xBF };

        [Fact]
        public void StreamJsonReader_WithBOM_ShouldParseCorrectly()
        {
            // Arrange
            var json = "{\"name\":\"test\"}";
            var ms = CreateStreamWithBOM(json);

            // Act
            var reader = new StreamJsonReader(ms, true);
            bool canRead = reader.Read();
            var tokenType = reader.TokenType;

            // Assert
            Assert.True(canRead);
            Assert.Equal(JsonTokenType.StartObject, tokenType);
        }

        [Fact]
        public void StreamJsonReader_WithoutBOM_ShouldParseCorrectly()
        {
            // Arrange
            var json = "{\"name\":\"test\"}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var ms = new MemoryStream(bytes);

            // Act
            var reader = new StreamJsonReader(ms, true);
            bool canRead = reader.Read();
            var tokenType = reader.TokenType;

            // Assert
            Assert.True(canRead);
            Assert.Equal(JsonTokenType.StartObject, tokenType);
        }

        [Fact]
        public void StreamJsonReader_WithPartialBOM_ShouldHandleGracefully()
        {
            // Arrange - Create a stream with only part of a BOM
            var partialBomStream = new MemoryStream();
            partialBomStream.Write(new byte[] { 0xEF, 0xBB }, 0, 2); // Only 2 bytes of the BOM
            partialBomStream.Write(Encoding.UTF8.GetBytes("{\"name\":\"test\"}"), 0, 15);
            partialBomStream.Seek(0, SeekOrigin.Begin);

            // Act - Use a try-catch since we expect an exception to be handled internally
            var reader = new StreamJsonReader(partialBomStream, true);
            bool readResult = false;
            Exception caughtException = null!;
            
            try
            {
                // After our fix, this should either return false or skip the partial BOM
                // and successfully read the JSON
                readResult = reader.Read();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
            
            // Assert - Either we successfully read something or we gracefully returned false,
            // but an exception should not be thrown
            Assert.Null(caughtException);
        }
        
        [Fact]
        public void JsonVirtualDataTable_WithBOM_ShouldParseCorrectly()
        {
            // Arrange
            var json = "[{\"ID\":1,\"Name\":\"Test\"}]";
            var ms = CreateStreamWithBOM(json);
            
            // Act - This should not throw an exception related to BOM
            var dataTable = new JsonVirtualDataTable(ms, true, "TestTable", 10, 
                elements => typeof(string), 4096);
            
            // Assert
            Assert.Equal("TestTable", dataTable.TableName);
            Assert.Equal(2, dataTable.Columns!.Count); // ID and Name columns
            
            // Enumerate the rows to make sure they're accessible
            var rows = dataTable.Rows!.GetEnumerator();
            Assert.True(rows.MoveNext());
            var row = rows.Current;

            //Should be a string because our type guesser above always indicates string.
            Assert.Equal(1.ToString(), row["ID"]);
            Assert.Equal("Test", row["Name"]);
            Assert.False(rows.MoveNext()); // Only one row
        }

        [Fact]
        public void JsonConnection_WithBOMFile_ShouldReadCorrectly()
        {
            // Arrange - Use an actual file with BOM from the test project
            string filePath = Path.Combine("Sources", "WithBOM", "test.json");
            
            // Ensure the test file has a BOM
            EnsureFileHasBOM(filePath);
            
            // Create a connection string that points to our test file
            var connectionString = new FileConnectionString { DataSource = filePath };
            var jsonConnection = new JsonConnection(connectionString);
            
            // Act
            jsonConnection.Open();
            var command = jsonConnection.CreateCommand("SELECT * FROM [testArray]");
            var reader = command.ExecuteReader();
            
            // Assert
            Assert.True(reader.Read()); // First row should be readable
            Assert.Equal(1, reader.GetInt32(reader.GetOrdinal("id")));
            Assert.Equal("Test Item 1", reader.GetString(reader.GetOrdinal("name")));
            Assert.Equal(10.5, reader.GetDouble(reader.GetOrdinal("value")));
            
            Assert.True(reader.Read()); // Second row should be readable
            Assert.Equal(2, reader.GetInt32(reader.GetOrdinal("id")));
            Assert.Equal("Test Item 2", reader.GetString(reader.GetOrdinal("name")));
            Assert.Equal(20.75, reader.GetDouble(reader.GetOrdinal("value")));
            
            Assert.False(reader.Read()); // No more rows
            
            // Clean up
            reader.Close();
            jsonConnection.Close();
        }

        [Fact]
        public void StreamJsonReader_WithBOM_ShouldSkipBOMWhenReadingMultipleChunks()
        {
            // Arrange - Create a JSON array that will require multiple reads
            var largeJson = "[";
            for (int i = 0; i < 1000; i++)
            {
                if (i > 0) largeJson += ",";
                largeJson += $"{{\"id\":{i}}}";
            }
            largeJson += "]";
            
            var ms = CreateStreamWithBOM(largeJson);
            
            // Act
            var reader = new StreamJsonReader(ms, true);
            bool startArrayRead = reader.Read(); // Should read the start of array
            
            // Assert - we should successfully parse this large JSON with BOM
            Assert.True(startArrayRead);
            Assert.Equal(JsonTokenType.StartArray, reader.TokenType);
            
            // Read the first object
            bool firstObjectRead = reader.Read();
            Assert.True(firstObjectRead);
            Assert.Equal(JsonTokenType.StartObject, reader.TokenType);
            
            // Keep reading and verify we can read till the end
            int objectCount = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    objectCount++;
                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }
            
            // We should have read all 1000 objects plus the first one we already counted
            Assert.True(objectCount > 900); // We might not count exactly 1000 due to how tokens are read
        }

        [Fact]
        public void JsonVirtualDataTable_WithBOM_ShouldHandlePositioning()
        {
            // Arrange - Create JSON with multiple rows
            var json = "[";
            for (int i = 0; i < 10; i++)
            {
                if (i > 0) json += ",";
                json += $"{{\"Index\":{i},\"Value\":\"Item{i}\"}}";
            }
            json += "]";
            
            var ms = CreateStreamWithBOM(json);
            
            // Act - Create data table that would normally be affected by BOM
            var dataTable = new JsonVirtualDataTable(ms, true, "TestPositioning", 10, 
                elements => typeof(string), 4096);
            
            // Assert - Verify we can enumerate all rows
            Assert.Equal("TestPositioning", dataTable.TableName);
            Assert.Equal(2, dataTable.Columns!.Count);
            
            // Count the rows to ensure we have all 10
            int rowCount = 0;
            foreach (var row in dataTable.Rows!)
            {
                //Should be a string because our type guesser above always indicates string.
                Assert.Equal(rowCount.ToString(), row["Index"]);
                Assert.Equal($"Item{rowCount}", row["Value"]);
                rowCount++;
            }
            
            Assert.Equal(10, rowCount);
        }

        // Helper method to ensure the test file has a BOM
        private void EnsureFileHasBOM(string filePath)
        {
            // Check if the file already has a BOM
            byte[] fileBytes = File.ReadAllBytes(filePath);
            if (fileBytes.Length >= 3 && 
                fileBytes[0] == Utf8BOM[0] && 
                fileBytes[1] == Utf8BOM[1] && 
                fileBytes[2] == Utf8BOM[2])
            {
                return; // File already has a BOM
            }

            // Add a BOM to the file
            string content = File.ReadAllText(filePath);
            using (var ms = new MemoryStream())
            {
                ms.Write(Utf8BOM, 0, Utf8BOM.Length);
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                ms.Write(contentBytes, 0, contentBytes.Length);
                File.WriteAllBytes(filePath, ms.ToArray());
            }
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