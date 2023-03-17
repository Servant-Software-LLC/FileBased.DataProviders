//using System.Data;
//using System.Data.JsonClient;
//using Xunit;

//namespace Data.Json.Tests;

//public class DataAdapterTests
//{
//    /// <summary>
//    /// Unit test created based on "Using the Sample .NET Framework Data Provider"
//    /// REF: https://learn.microsoft.com/en-us/previous-versions/aa720697(v=vs.71)#using-the-sample-net-framework-data-provider
//    /// </summary>
//    [Fact]
//    public void Fill_and_Update()
//    {
//        using (var conn = new JsonConnection($"Data Source=./Sources/Folder;"))
//        {
//            conn.Open();

//            var adapter = new JsonDataAdapterTests();
//            adapter.SelectCommand = new JsonCommand("select * from locations", conn);
//            //adapter.UpdateCommand = new JsonCommand("update city, state, zip values(@city, @state, @zip) where id = @id", conn);
//            //adapter.UpdateCommand.Parameters.Add("@city", DbType.String);
//            //adapter.UpdateCommand.Parameters.Add("@state", DbType.String);
//            //adapter.UpdateCommand.Parameters.Add("@zip", DbType.Int64);
//            //adapter.UpdateCommand.Parameters.Add("@id", DbType.Int64);

//            DataSet ds = new DataSet();
//            adapter.Fill(ds, "locations");

//            //TODO: Validate the rows in the DataTable of the DataSet.

//            ds.Tables["locations"].Rows[1]["zip"] = 78130;
//            adapter.Update(ds, "locations");

//            //Read the updated JSON from disk in order to valid success.
//            using (var cmd = new JsonCommand("select * from locations", conn))
//            using (var reader = cmd.ExecuteReader())
//            {
//                //Read first row
//                var success = reader.Read();
//                Assert.True(success);

//                //Read second row
//                success = reader.Read();
//                Assert.True(success);

//                var zip = reader["zip"];
//                Assert.IsType<string>(zip);
//                Assert.Equal(78130, zip);
//            }
//        }
//    }
//}