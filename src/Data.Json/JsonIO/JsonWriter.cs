namespace Data.Json.JsonIO
{

    internal abstract class JsonWriter : JsonDataAccess
    {
        private readonly JsonCommand command;
        public JsonWriter(JsonCommand command,JsonDocument jsonDocument, JsonQueryParser jSONQuery) : base(jsonDocument, jSONQuery)
        {
            this.command = command;
        }
        public override IEnumerable<string> GetTables()
        {
            return jsonDocument.RootElement.EnumerateObject().Select(x=>x.Name);
        }
        public abstract int Execute();
       
        public bool Save()
        {
        JsonConnection.LockSlim.EnterWriteLock();
            using (var fileStream = new FileStream(command.Connection.ConnectionString, FileMode.Create, FileAccess.Write))
            using (var jsonWriter = new Utf8JsonWriter(fileStream))
            {
                jsonWriter.WriteStartObject();
                foreach (DataTable table in base.DataSet.Tables)
                {
                    jsonWriter.WriteStartArray(table.TableName);
                    foreach (DataRow row in table.Rows)
                    {
                        jsonWriter.WriteStartObject();
                        foreach (DataColumn column in table.Columns)
                        {
                            var dataType = column.DataType.Name;
                            if (row.IsNull(column.ColumnName))
                            {
                                dataType = "Null";
                            }
                            switch (dataType)
                            {
                                case "Decimal":
                                    jsonWriter.WriteNumber(column.ColumnName, (decimal)row[column]);
                                    break;
                                case "String":
                                    jsonWriter.WriteString(column.ColumnName, row[column].ToString().AsSpan());
                                    break;
                                case "Boolean":
                                    jsonWriter.WriteBoolean(column.ColumnName, (bool)row[column]);
                                    break;
                                case "Null":
                                    jsonWriter.WriteNull(column.ColumnName);
                                    break;
                                default:
                                    throw new NotSupportedException($"Data type {column.DataType.Name} is not supported.");
                            }

                            //jsonWriter.WriteString(column.ColumnName, row[column].ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                    jsonWriter.WriteEndArray();
                }
                jsonWriter.WriteEndObject();
            }
                JsonConnection.LockSlim.ExitWriteLock();

            return true;
        }
    }
}
