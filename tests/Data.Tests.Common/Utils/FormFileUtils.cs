using Microsoft.AspNetCore.Http;
using Moq;

namespace Data.Tests.Common.Utils;

public static class FormFileUtils
{
    public static IFormFile MockFormFile(string filePath, string contentType)
    {
        // Open the file as a stream
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        // Create a mock of IFormFile
        var mockFormFile = new Mock<IFormFile>();

        // Set up the mock to return the necessary values from the real file
        mockFormFile.Setup(f => f.FileName).Returns(Path.GetFileName(filePath));
        mockFormFile.Setup(f => f.Length).Returns(fileStream.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(fileStream);
        mockFormFile.Setup(f => f.ContentType).Returns($"text/{contentType}");
        mockFormFile.Setup(f => f.ContentDisposition).Returns($"form-data; name=\"file\"; filename=\"{Path.GetFileName(filePath)}\"");

        return mockFormFile.Object;
    }
}
