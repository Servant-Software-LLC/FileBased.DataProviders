using Data.Tests.Common;
using System.Data;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public partial class XmlTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldInsertDataIntoDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldDeleteDataFromDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldUpdateDataInDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDBConnectionString);
        connection.Open();

        // Start a transaction
        var transaction = (XmlTransaction)connection.BeginTransaction();

        try
        {
            // Create a command to insert data into the locations table
            var command = new XmlCommand("INSERT INTO locations (city, state) VALUES (@City, @State)", connection, transaction);

            command.Parameters.Add(new XmlParameter("City", "Bannu"));
            command.Parameters.Add(new XmlParameter("State", "MA"));

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
            var adapter = new XmlDataAdapter("SELECT * FROM locations WHERE city = 'Bannu'", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            Assert.Equal(0, dataSet.Tables[0].Rows.Count);
        }

        // Close the connection
        connection.Close();
    }


}
