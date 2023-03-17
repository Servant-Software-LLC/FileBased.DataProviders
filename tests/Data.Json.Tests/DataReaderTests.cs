//using System.Data;
//using System.Data.JsonClient;
//using Xunit;

//namespace Data.Json.Tests;

//public class DataReaderTests
//{
//    /// <summary>
//    /// Unit test created based on "Using the Sample .NET Framework Data Provider"
//    /// REF: https://learn.microsoft.com/en-us/previous-versions/aa720697(v=vs.71)#using-the-sample-net-framework-data-provider
//    /// </summary>
//    [Fact]
//    public void ExecuteReader_Basic()
//    {
//        using (var conn = new JsonConnection($"Data Source=Sources/database.json;"))
//        {
//            conn.Open();

//            using (var cmd = new JsonCommand("select * from employees", conn))
//            using (var reader = cmd.ExecuteReader())
//            {
//                //Read first row
//                var success = reader.Read();
//                Assert.True(success);
                



//                //Read second row
//                success = reader.Read();
//                Assert.True(success);
//                ValidateRow(reader, "Bob", "bob32@gmail.com", 95000, false);

//                //No more rows to read
//                success = reader.Read();
//                Assert.False(success);

//                reader.Close();
//            }
//            conn.Close();
//        }
//    }

//    private void ValidateRow(IDataReader reader, string expectedName, string expectedEmail, 
//                             decimal expectedSalary, bool expectedMarried)
//    {
//        Assert.Equal(4, reader.FieldCount);

//        var name = reader["name"];
//        Assert.IsType<string>(name);
//        Assert.Equal(expectedName, name);

//        var email = reader["email"];
//        Assert.IsType<string>(email);
//        Assert.Equal(expectedEmail, email);

//        var salary = reader["salary"];
//        Assert.IsType<decimal>(salary);
//        Assert.Equal(expectedSalary, salary);

//        var married = reader["married"];
//        Assert.IsType<bool>(married);
//        Assert.Equal(expectedMarried, (bool)married);
//    }
//}
