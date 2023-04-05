using Data.Common.Extension;
using Data.Json.Tests.FileAsDatabase;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase
{
    public partial class CsvDataAdapterTests
    {
        [Fact]
        public void DataAdapter_ShouldFillTheDataSet()
        {
            DataAdapterTests.DataAdapter_ShouldFillTheDataSet(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void Adapter_ShouldReturnData()
        {
            DataAdapterTests.Adapter_ShouldReturnData(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB), true);
        }

        [Fact]
        public void DataAdapter_ShouldFillTheDataSet_WithFilter()
        {
            DataAdapterTests.DataAdapter_ShouldFillTheDataSet_WithFilter(
                           () => new CsvConnection(ConnectionStrings.Instance.
                           FolderAsDB), true);
        }

        [Fact]
        public void Adapter_ShouldFillDatasetWithInnerJoinFromFolderAsDB()
        {
            DataAdapterTests.Adapter_ShouldFillDatasetWithInnerJoin(
                    () => new CsvConnection(ConnectionStrings.Instance.eComFolderDB));
        }

        [Fact]
        public void Adapter_ShouldReadDataWithSelectedColumns()
        {
            DataAdapterTests.Adapter_ShouldReadDataWithSelectedColumns(
                         () => new CsvConnection(ConnectionStrings.Instance.
                         FolderAsDB));
        }

        [Fact]
        public void Update_DataAdapter_Should_Update_Existing_Row()
        {
            var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
            DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row(
                () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
            , true);
        }

        [Fact]
        public void FillSchema_ShouldReturnDataTableWithAllColumns()
        {
            DataAdapterTests.FillSchema_ShouldReturnDataTableWithAllColumns(
                 () => new CsvConnection(ConnectionStrings.Instance.
                 FolderAsDB));
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull()
        {
            DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull(
                  () => new CsvDataAdapter());
        }

        [Fact]
        public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
        {
            DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull(
                 () => new CsvConnection(ConnectionStrings.Instance.
                 FolderAsDB));
        }

        [Fact]
        public void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty()
        {
            DataAdapterTests.CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters(
                  () => new CsvConnection(ConnectionStrings.Instance.
                  FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }

        [Fact]
        public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
        {
            DataAdapterTests.GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery(
                   () => new CsvConnection(ConnectionStrings.Instance.
                   FolderAsDB));
        }
    }
}