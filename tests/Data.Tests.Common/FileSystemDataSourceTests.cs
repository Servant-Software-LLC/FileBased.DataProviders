using Data.Common.DataSource;
using Data.Common.Interfaces;
using Moq;
using Xunit;

namespace Data.Tests.Common;

public class FileSystemDataSourceTests
{
    [Theory]
    [InlineData("../../etc/passwd")]
    [InlineData("../secret")]
    [InlineData("foo/../../bar")]
    public void StorageExists_PathTraversal_ThrowsArgumentException(string tableName)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            var mockConn = new Mock<IFileConnection>();
            mockConn.Setup(c => c.Database).Returns(tempDir);
            mockConn.Setup(c => c.FileExtension).Returns("csv");
            mockConn.Setup(c => c.FolderAsDatabase).Returns(true);

            var ds = new FileSystemDataSource(tempDir, DataSourceType.Directory, "csv", mockConn.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ds.StorageExists(tableName));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Theory]
    [InlineData("../../etc/passwd")]
    [InlineData("../secret")]
    public void DeleteStorage_PathTraversal_ThrowsArgumentException(string tableName)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            var mockConn = new Mock<IFileConnection>();
            mockConn.Setup(c => c.Database).Returns(tempDir);
            mockConn.Setup(c => c.FileExtension).Returns("csv");
            mockConn.Setup(c => c.FolderAsDatabase).Returns(true);

            var ds = new FileSystemDataSource(tempDir, DataSourceType.Directory, "csv", mockConn.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ds.DeleteStorage(tableName));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void StorageExists_ValidTableName_DoesNotThrow()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            var mockConn = new Mock<IFileConnection>();
            mockConn.Setup(c => c.Database).Returns(tempDir);
            mockConn.Setup(c => c.FileExtension).Returns("csv");
            mockConn.Setup(c => c.FolderAsDatabase).Returns(true);

            var ds = new FileSystemDataSource(tempDir, DataSourceType.Directory, "csv", mockConn.Object);

            // Act & Assert — should not throw
            var result = ds.StorageExists("valid_table");
            Assert.False(result); // file doesn't exist but no exception
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
