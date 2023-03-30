using Data.Common.Extension;
using Data.Tests.Common;
using System.Data;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public partial class CsvTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldInsertDataIntoDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldDeleteDataFromDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldUpdateDataInDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.Instance.FolderAsDB);
        connection.Open();

        // Start a transaction
        var transaction = (CsvTransaction)connection.BeginTransaction();

        try
        {
            // Create a command to insert data into the locations table
            var command = new CsvCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

            command.Parameters.Add(new CsvParameter("City", "Bannu"));
            command.Parameters.Add(new CsvParameter("State", "MA"));

            // Execute the command
            int rowsAffected = command.ExecuteNonQuery();

            // Simulate an exception by dividing by zero
            int x = 0;
            int y = 1 / x;

            // If an exception is not thrown, the test will fail
            Assert.True(false, "Exception was not thrown");

            // Commit the transaction
            transaction.Commit();
        }
        catch (Exception)
        {
            // Rollback the transaction
            transaction.Rollback();

            // Query the locations table to verify that the data was not inserted
            var adapter = new CsvDataAdapter("SELECT * FROM locations WHERE city = 'Bannu'", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(0, dataSet.Tables[0].Rows.Count);
        }

        // Close the connection
        connection.Close();
    }

}
