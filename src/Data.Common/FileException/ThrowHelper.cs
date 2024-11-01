﻿using Data.Common.DataSource;

namespace Data.Common.FileException;

internal static class ThrowHelper
{
    private const string QUERY_NOT_SUPPORTED = "The query is not supported";
    //private const string TABLE_NOT_FOUND = "{0} Not found";
    //private const string COLUMN_NOT_FOUND = "{0} Not found";
    //private const string INVALID_CON_STRING = "The connection string is not a valid connection string. It should be like 'DataSource=<FilePath>;'";

    public static void ThrowIfInvalidPath(DataSourceType pathType, string pathToDatabase)
    {
        if (pathType==DataSourceType.None)
        {
            throw new InvalidConnectionStringException(pathToDatabase);
        }
    }
    
    public static QueryNotSupportedException GetQueryNotSupportedException() => new(QUERY_NOT_SUPPORTED);

    public static void ThrowQuerySyntaxException(string error, string command) => 
        throw new QuerySyntaxException($"{error}. Command = {command}");

    //public static void ThrowTableNotFoundException(string tableName) =>
    //    throw new TableNotFoundException(string.Format(TABLE_NOT_FOUND,tableName));

    //public static void ThrowColumnNotFoundException(string columnName) =>
    //    throw new ColumnNotFoundException(string.Format(COLUMN_NOT_FOUND, columnName));

    //internal static void ThrowInvalidConnectionString() =>
    //    throw new InvalidConnectionStringException(INVALID_CON_STRING);

    internal static void ThrowIfNotSupportedAggregateFunctionException(string aggregateName)
    {
        if (aggregateName.ToLower() != "count")
        {
            throw new NotImplementedException($"The aggregate function {aggregateName} is not yet implemented.");
        }
    }

    internal static void ThrowIfNotAsterik() => throw new NotImplementedException($"Count by a specific column is not yet implemented.");
}
