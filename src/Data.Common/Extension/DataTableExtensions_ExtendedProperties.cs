//using SqlBuildingBlocks.Utils;

namespace Data.Common.Extension;

//public static class DataTableExtensions_ExtendedProperties
//{
//    public class DataTableExProps : ExProps<DataTableExProps>
//    {
//        private readonly DataTable dataTable;

//        public DataTableExProps(DataTable dataTable) : base(() => dataTable.ExtendedProperties) 
//        {
//            this.dataTable = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
//        }

//        public bool FullTableInMemory
//        {
//            get => GetValueType<bool>(nameof(FullTableInMemory));
//        }

//        public IEnumerable<DataRow> SetDataSource(IEnumerable<DataRow> dataSource, int pageSize)
//        {
//            //Assume that the data source has rows until otherwise 
//            Set(nameof(FullTableInMemory), false);

            
//        }


//    }

//    public static DataTableExProps ExProps(this DataTable dataTable) => new DataTableExProps(dataTable);
//}

