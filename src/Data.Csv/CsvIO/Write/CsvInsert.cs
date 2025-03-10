﻿using System.Data.CsvClient;

namespace Data.Csv.CsvIO.Write;

internal class CsvInsert : Common.FileIO.Write.FileInsertWriter
{
    public CsvInsert(FileInsert fileStatement, FileConnection<CsvParameter> fileConnection, FileCommand<CsvParameter> fileCommand) 
        : base(fileStatement, fileConnection, fileCommand)
    {
    }

    public override bool SchemaUnknownWithoutData => true;

    protected override void RealizeSchema(DataTable dataTable)
    {
        // Update the data type of the columns in the data table, now that they can be determined.

        if (fileStatement is not FileInsert fileInsertStatement)
            throw new Exception($"Expected {nameof(fileStatement)} to be a {nameof(FileInsert)}");

        // Determine if column data types can now be inferred from the data.
        foreach (var val in fileInsertStatement.GetValues())
        {
            var dataColumnType = GetDataColumnType(val.Value);

            // The column should already be here.
            if (dataTable.Columns.Contains(val.Key))
            {
                //Update the data type of the column.
                dataTable.Columns[val.Key].DataType = dataColumnType;
            }

            foreach (var columnNameHint in fileInsertStatement.ColumnNameHints)
            {
                //Check to see if we need to change the data type of the column
                if (ColumnNameIndicatesIdentity(columnNameHint))
                {
                    if (dataTable.Columns.Contains(columnNameHint))
                    {
                        dataTable.Columns[columnNameHint].DataType = fileConnection.PreferredFloatingPointDataType.ToType();
                    }
                }
            }
        }
    }
}
