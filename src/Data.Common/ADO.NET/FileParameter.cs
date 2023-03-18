namespace System.Data.FileClient;

public class FileParameter : IDbDataParameter
{
    object? m_value;

    public FileParameter()
        :this(string.Empty, DbType.Object)
    {
    }

    public FileParameter(string parameterName, DbType type)
    {
        ParameterName = parameterName;
        DbType = type;
    }

    public FileParameter(string parameterName, object value)
    {
        ParameterName = parameterName;
        Value = value;   // Setting the value also infers the type.
    }

    public FileParameter(string parameterName, DbType dbType, string sourceColumn)
    {
        ParameterName = parameterName;
        DbType = dbType;
        SourceColumn = sourceColumn;
    }

    public DbType DbType { get; set; } = DbType.Object;

    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

    public bool IsNullable => false;

    public string ParameterName { get; set; }

    public string SourceColumn { get; set; } = string.Empty;

    public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

    public object Value
    {
        get
        {
            return m_value!;
        }
        set
        {
            m_value = value;
            DbType = InferType(value);
        }
    }

    public byte Precision { get; set; }
    public byte Scale { get ; set; }
    public int Size { get ; set; }

    private static DbType InferType(object value)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Empty:
                throw new SystemException("Invalid data type");

            case TypeCode.Object:
                return DbType.Object;

            case TypeCode.DBNull:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                // Throw a SystemException for unsupported data types.
                throw new SystemException("Invalid data type");

            case TypeCode.Boolean:
                return DbType.Boolean;

            case TypeCode.Byte:
                return DbType.Byte;

            case TypeCode.Int16:
                return DbType.Int16;

            case TypeCode.Int32:
                return DbType.Int32;

            case TypeCode.Int64:
                return DbType.Int64;

            case TypeCode.Single:
                return DbType.Single;

            case TypeCode.Double:
                return DbType.Double;

            case TypeCode.Decimal:
                return DbType.Decimal;

            case TypeCode.DateTime:
                return DbType.DateTime;

            case TypeCode.String:
                return DbType.String;

            default:
                throw new SystemException("Value is of unknown data type");
        }
    }
}