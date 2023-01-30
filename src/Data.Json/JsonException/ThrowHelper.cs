namespace Data.Json.JsonException
{
    internal static class ThrowHelper
    {
        private const string FILE_NOT_FOUND = "Database not found";
        private const string QUERY_NOT_SUPPORTED = "The query is not supported";
        private const string SYNTAX_ERROR = "Syntax error";
        private const string TABLE_NOT_FOUND = "{0} Not found";
        private const string COLUMN_NOT_FOUND = "{0} Not found";
        private const string INVALID_CON_STRING = "The connection string is not a valid connection string. It should be like 'DataSource=<FilePath>;'";
        private const string INVALID_JSON = "The json file is not valid";
        public static void ThrowIfFileNotExist(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException(FILE_NOT_FOUND);
            }
        }
        public static void ThrowIfInvalidJson(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.ValueKind!=JsonValueKind.Object)
            {
                throw new InvalidJsonFileException(INVALID_JSON);
            }
        }
        public static void ThrowQueryNotSupportedException()
        {
          throw new QueryNotSupportedException(QUERY_NOT_SUPPORTED);
        }
        public static void ThrowSyntaxtErrorException(string error)
        {
            throw new QueryNotSupportedException($"{SYNTAX_ERROR} {error}");
        }
        public static void ThrowTableNotFoundException(string tableName)
        {
            throw new TableNotFoundException(string.Format(TABLE_NOT_FOUND,tableName));
        }
        public static void ThrowColumnNotFoundException(string columnName)
        {
            throw new ColumnNotFoundException(string.Format(COLUMN_NOT_FOUND, columnName));
        }

        internal static void ThrowIfInvalidConnectionString()
        {
            throw new InvalidConnectionString(string.Format(INVALID_CON_STRING));

        }
    }
}
