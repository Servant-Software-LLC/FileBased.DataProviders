using System.Data;
using System.Data.Common;

namespace Data.Json;

public class JsonDataAdapter : DbDataAdapter, IDataAdapter
{
    public JsonCommand SelectCommand { get; set; }
    public JsonCommand UpdateCommand { get; set; }

    public MissingMappingAction MissingMappingAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public MissingSchemaAction MissingSchemaAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ITableMappingCollection TableMappings => throw new NotImplementedException();

    public int Fill(DataSet dataSet)
    {
        throw new NotImplementedException();
    }

    public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
    {
        throw new NotImplementedException();
    }

    public IDataParameter[] GetFillParameters()
    {
        throw new NotImplementedException();
    }

    public int Update(DataSet dataSet)
    {
        throw new NotImplementedException();
    }
}
