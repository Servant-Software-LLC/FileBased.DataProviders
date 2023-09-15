namespace System.Data.FileClient;

public abstract class FileParameter<TFileParameter> : DbParameter, IDbDataParameter, ICloneable
    where TFileParameter : FileParameter<TFileParameter>
{
    object? m_value;

    public FileParameter()
        :this(string.Empty, DbType.Object)
    {
    }

    public FileParameter(string parameterName, DbType type)
    {
        ParameterName = StandardizeName(parameterName);
        DbType = type;
    }

    public FileParameter(string parameterName, object value)
    {
        ParameterName = StandardizeName(parameterName);
        Value = value;   // Setting the value also infers the type.
    }

    public FileParameter(string parameterName, DbType dbType, string sourceColumn)
    {
        ParameterName = StandardizeName(parameterName);
        DbType = dbType;
        SourceColumn = sourceColumn;
    }

    public override DbType DbType { get; set; } = DbType.Object;

    public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;


    /// <summary>
    /// Internally this value is ignored.
    /// Explanation of use: https://stackoverflow.com/questions/1075863/dbparameter-isnullable-functionality
    /// </summary>
    public override bool IsNullable { get; set; }

    public override string ParameterName { get; set; }

    public override string SourceColumn { get; set; } = string.Empty;

    public override bool SourceColumnNullMapping { get; set; }

    public override DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

    public override object Value
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

    public override byte Precision { get; set; }
    public override byte Scale { get ; set; }
    public override int Size { get ; set; }

    public override void ResetDbType() => DbType = DbType.Object;

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

    public abstract TFileParameter Clone();
    object ICloneable.Clone() => Clone();

    //Be forgiving, if the name isn't prefixed with a '@', then add it.
    private string StandardizeName(string name) => name.StartsWith('@') ? name : $"@{name}";
}