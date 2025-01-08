namespace System.Data.FileClient;


/// <summary>
/// Represents a parameter for a file operation.
/// </summary>
/// <typeparam name="TFileParameter">The type of the file parameter.</typeparam>
public abstract class FileParameter<TFileParameter> : DbParameter, IDbDataParameter, ICloneable
    where TFileParameter : FileParameter<TFileParameter>
{
    object m_value;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileParameter{TFileParameter}"/> class with default values.
    /// </summary>
    public FileParameter()
        : this(string.Empty, DbType.Object)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileParameter{TFileParameter}"/> class with the specified name and data type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="type">The data type of the parameter.</param>
    public FileParameter(string parameterName, DbType type)
    {
        ParameterName = StandardizeName(parameterName);
        DbType = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileParameter{TFileParameter}"/> class with the specified name and value.
    /// The data type is inferred from the value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public FileParameter(string parameterName, object value)
    {
        ParameterName = StandardizeName(parameterName);
        Value = value;   // Setting the value also infers the type.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileParameter{TFileParameter}"/> class with the specified name, data type, and source column.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">The data type of the parameter.</param>
    /// <param name="sourceColumn">The source column of the parameter.</param>
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
    public override byte Scale { get; set; }
    public override int Size { get; set; }

    public override void ResetDbType() => DbType = DbType.Object;

    private static DbType InferType(object value)
    {
        var typeCode = Type.GetTypeCode(value.GetType());
        switch (typeCode)
        {
            case TypeCode.Empty:
                throw new SystemException($"Invalid data type for type code {typeCode}");

            case TypeCode.Object:
                return DbType.Object;

            case TypeCode.DBNull:
                return DbType.Object;  //We have no indication of type, so default to Object.

            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                // Throw a SystemException for unsupported data types.
                throw new SystemException($"Invalid data type for type code {typeCode}");

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
                throw new SystemException($"Value of type code {typeCode} is of unknown data type");
        }
    }

    /// <summary>
    /// Clones this instance of the file parameter.
    /// </summary>
    /// <returns>A new instance of the file parameter with the same values as this instance.</returns>
    public abstract TFileParameter Clone();

    /// <summary>
    /// Clones this instance of the file parameter.
    /// </summary>
    /// <returns>A new instance of the file parameter with the same values as this instance.</returns>
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Standardizes the name of the parameter by ensuring it starts with a '@'.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <returns>The standardized name of the parameter.</returns>
    private string StandardizeName(string name) => name.StartsWith("@") ? name : $"@{name}";
}
