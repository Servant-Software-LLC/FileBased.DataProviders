using SqlBuildingBlocks.POCOs;
using System.Text;


namespace Data.Common.Extension;

public static class VirtualDataTableExtensions
{
    public static void IntegrityCheck(this VirtualDataTable table)
    {
        StringBuilder stringBuilder = new();
        DataTable lastDataTableRef = null;

        //Check columns
        foreach (DataColumn column in table.Columns)
        {
            if (lastDataTableRef != null)
            {
                if (!object.ReferenceEquals(column.Table, lastDataTableRef))
                {
                    stringBuilder.AppendLine($"Column {column.ColumnName}'s DataTable doesn't ref match.");
                }
            }
            lastDataTableRef = column.Table;
        }

        //Check rows
        int rowNumber = 0;
        foreach (DataRow dataRow in table.Rows)
        {
            if (!object.ReferenceEquals(dataRow.Table, lastDataTableRef))
            {
                stringBuilder.AppendLine($"Row #{rowNumber}'s DataTable doesn't ref match.");
            }

            rowNumber++;
            lastDataTableRef = dataRow.Table;
        }

        if (stringBuilder.Length > 0)
            throw new Exception(stringBuilder.ToString());
    }

    public static void IntegrityCheck(this VirtualDataTable table, DataRow row)
    {
        DataTable lastDataTableRef = table.Columns.Cast<DataColumn>().First().Table;

        //Check new row
        if (!object.ReferenceEquals(row.Table, lastDataTableRef))
        {
            throw new Exception("Row's DataTable doesn't ref match.");
        }
    }
}
