using Data.Json.Utils;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Data;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Data.Json.Tests;

public class JsonVirtualDataTableTests
{
    private readonly ITestOutputHelper logger;

    public JsonVirtualDataTableTests(ITestOutputHelper logger)
    {
        this.logger = logger;
    }


    [Fact]
    public void Columns_DefaultGuessTypeFunc_RowsLessThanGuessRows()
    {
        //Extra things validated:
        //  Different number of properties on objects in array (Extra 'Company' property in one)
        //  String contains an apostrophe
        //  Determines dates
        //  Property of type boolean in one object and type double in another (should be string)

        //Arrange
        const string jsonContent = @"
[
  {
    ""Index"": 1,
    ""Last Name"": ""Maltby"",
    ""BirthDate"": ""04/11/1993 12:00:00 AM"",
    ""IsEmployed"": true,
    ""IsHappy"": true
  },
  {
    ""Index"": 2,
    ""Last Name"": ""O'Brien"",
    ""BirthDate"": ""09/12/1995 12:00:00 AM"",
    ""IsEmployed"": false,
    ""Company"": ""Bobson"",
    ""IsHappy"": 0
  }
]";
        var stream = GetStream(jsonContent);
        
        //Act:  Columns are determined in the ctor.
        JsonVirtualDataTable table = new(stream, "MyTable", 10, null, 4096);

        //Assert
        Assert.NotNull(table.Columns);
        Assert.Equal(6, table.Columns.Count);

        //Check columns
        AssertColumn(table.Columns, "Index", typeof(double));
        AssertColumn(table.Columns, "Last Name", typeof(string));
        AssertColumn(table.Columns, "BirthDate", typeof(DateTime));
        AssertColumn(table.Columns, "IsEmployed", typeof(bool));
        AssertColumn(table.Columns, "Company", typeof(string));
        AssertColumn(table.Columns, "IsHappy", typeof(string));
    }

    [Fact]
    public void ColumnsAndRows_DefaultGuessTypeFunc_RowsMoreThanGuessRows_ButExtraRowsHaveExtraProperties_ExtraPropertiesAreIgnored()
    {
        const string jsonContent = @"
[
  {
    ""Index"": 1,
    ""Last Name"": ""Maltby"",
    ""BirthDate"": ""04/11/1993 12:00:00 AM"",
    ""IsEmployed"": true,
  },
  {
    ""Index"": 2,
    ""Last Name"": ""O'Brien"",
    ""BirthDate"": ""09/12/1995 12:00:00 AM"",
    ""IsEmployed"": false,
    ""Company"": ""Bobson""
  }
]";
        var stream = GetStream(jsonContent);

        //Act:  Columns are determined in the ctor.
        JsonVirtualDataTable table = new(stream, "MyTable", 1, null, 4096);

        //Assert
        Assert.NotNull(table.Columns);
        Assert.Equal(4, table.Columns.Count);

        //Should be able to pull both rows as well.
        Assert.NotNull(table.Rows);
        var rows = table.Rows.ToList();
        Assert.Equal(2, rows.Count);

        //First row
        var firstRow = rows[0];
        Assert.NotNull(firstRow);
        Assert.Equal(1d, firstRow["Index"]);
        Assert.Equal("Maltby", firstRow["Last Name"]);
        Assert.Equal(new DateTime(1993, 4, 11), firstRow["BirthDate"]);
        Assert.Equal(true, firstRow["IsEmployed"]);

        //Second row (Company should be missing from this row.)
        var secondRow = rows[1];
        Assert.NotNull(secondRow);
        Assert.Equal(4, secondRow.ItemArray.Length); 
        Assert.Equal(2d, secondRow["Index"]);
        Assert.Equal("O'Brien", secondRow["Last Name"]);
        Assert.Equal(new DateTime(1995, 9, 12), secondRow["BirthDate"]);
        Assert.Equal(false, secondRow["IsEmployed"]);
    }

    [Fact]
    public void Rows_WithEmbeddedArray_ReturnsAsString()
    {
        //Arrange
        const string jsonContent = @"
[
  {
    ""name"": ""Adeel Solangi"",
    ""language"": [""ab"", ""cd""],
  }
]";

        var stream = GetStream(jsonContent);

        //Act:  Columns are determined in the ctor.
        JsonVirtualDataTable table = new(stream, "MyTable", 10, null, 4096);

        //Assert
        Assert.NotNull(table.Columns);
        Assert.Equal(2, table.Columns.Count);
        AssertColumn(table.Columns, "name", typeof(string));
        AssertColumn(table.Columns, "language", typeof(string));

        Assert.NotNull(table.Rows);
        var row = table.Rows.First();
        Assert.Equal(2, row.ItemArray.Length);
        Assert.Equal("Adeel Solangi", row["name"]);
        Assert.Equal("[\"ab\", \"cd\"]", row["language"]);
    }

    [Fact]
    public void Rows_WithEmbeddedObject_ReturnsAsString()
    {
        // Arrange
        const string jsonContent = @"
[
    {
    ""name"": ""Adeel Solangi"",
    ""details"": { ""age"": 30, ""city"": ""Vienna"" }
    }
]";

        var stream = GetStream(jsonContent);

        // Act: Columns are determined in the constructor.
        JsonVirtualDataTable table = new(stream, "MyTable", 10, null, 4096);

        // Assert
        Assert.NotNull(table.Columns);
        Assert.Equal(2, table.Columns.Count);
        AssertColumn(table.Columns, "name", typeof(string));
        AssertColumn(table.Columns, "details", typeof(string));

        Assert.NotNull(table.Rows);
        var row = table.Rows.First();
        Assert.Equal(2, row.ItemArray.Length);
        Assert.Equal("Adeel Solangi", row["name"]);
        Assert.Equal(@"{ ""age"": 30, ""city"": ""Vienna"" }", row["details"]); // JSON object should be stored as a string
    }

    [Fact]
    public void Rows_SingleObjectOutsideOfArray_Success()
    {
        // Arrange
        const string jsonContent = @"
  {
    ""name"": ""George"",
    ""email"": ""GeorgeMaltby@hotmail.com"",
    ""salary"": 5000,
    ""married"": false
  }
";

        var stream = GetStream(jsonContent);

        // Act: Columns are determined in the constructor.
        JsonVirtualDataTable table = new(stream, "MyTable", 10, null, 4096);

        // Assert
        Assert.NotNull(table.Columns);
        Assert.Equal(4, table.Columns.Count);
        AssertColumn(table.Columns, "name", typeof(string));
        AssertColumn(table.Columns, "email", typeof(string));
        AssertColumn(table.Columns, "salary", typeof(double));
        AssertColumn(table.Columns, "married", typeof(bool));

        Assert.NotNull(table.Rows);
        var rows = table.Rows.ToList();
        Assert.Single(rows);
        var row = rows[0];
        Assert.Equal("George", row["name"]);
        Assert.Equal("GeorgeMaltby@hotmail.com", row["email"]);
        Assert.Equal(5000d, row["salary"]);
        Assert.Equal(false, row["married"]);
    }


    [Fact]
    public void Rows_RowPastGuessRowOfDifferentType_ShouldThrowError()
    {
        //Arrange
        const string jsonContent = @"
[
  {
    ""IsHappy"": true
  },
  {
    ""IsHappy"": 0
  }
]";
        var stream = GetStream(jsonContent);

        //Act:  Columns are determined in the ctor.
        JsonVirtualDataTable table = new(stream, "MyTable", 1, null, 4096);

        //Assert
        Assert.NotNull(table.Columns);
        Assert.Single(table.Columns);

        //Check columns
        AssertColumn(table.Columns, "IsHappy", typeof(bool));

        Assert.NotNull(table.Rows);
        var rowEnumerator = table.Rows.GetEnumerator();

        //Getting the first row should be fine.
        Assert.True(rowEnumerator.MoveNext());
        var firstRow = rowEnumerator.Current;
        Assert.Equal(true, firstRow["IsHappy"]);

        //Getting the second row should throw an exception.
        Assert.Throws<InvalidDataException>(() => rowEnumerator.MoveNext());

    }

    [Fact(Skip = "Don't run performance test")]
    public void Rows_PerformanceTest()
    {
        //Arrange
        var stream = File.OpenRead(Path.Combine("Sources", "Large", "Large_5MB.json"));

        // Start timer
        var stopwatch = Stopwatch.StartNew();

        //Act:  Columns are determined in the ctor.
        JsonVirtualDataTable table = new(stream, "MyTable", 10, null, 4096);

        const int maxRows = 100000;
        int rows = 0;
        foreach (var row in table.Rows!)
        {
            if (rows >= maxRows)
                break;
            rows++;

            //logger.WriteLine($"After {rows} rows Leftover length: {table.LeftoverLength}");
        }

        // Stop timer
        stopwatch.Stop();

        // Output execution time
        logger.WriteLine($"For {rows} the test execution time: {stopwatch.ElapsedMilliseconds} ms");

    }


    private void AssertColumn(DataColumnCollection columns, string columnName, Type columnType)
    {
        var column = columns[columnName];
        AssertColumn(column, columnName, columnType);
    }

    private void AssertColumn(DataColumn? column, string columnName, Type columnType)
    {
        Assert.NotNull(column);
        Assert.Equal(columnName, column.ColumnName);
        Assert.Equal(columnType, column.DataType);
    }

    private Stream GetStream(string jsonContent)
    {
        byte[] fileBytes = Encoding.UTF8.GetBytes(jsonContent);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        return fileStream;
    }
}
