﻿namespace Data.Csv.CsvIO.Write;

internal class CsvUpdate : FileUpdate
{
    public CsvUpdate(FileUpdateQuery queryParser, FileConnection FileConnection, FileCommand FileCommand) 
        : base(queryParser, FileConnection, FileCommand)
    {
        dataSetWriter = new CsvDataSetWriter(FileConnection, queryParser);
    }
}