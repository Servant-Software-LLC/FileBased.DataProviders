namespace EFCore.Common.Tests.Models;

/// <summary>
/// Entity with properties covering all mapped CLR types for type mapping verification.
/// </summary>
public class TypeMappingEntity
{
    public int Id { get; set; }

    // Original types
    public string StringValue { get; set; }
    public int IntValue { get; set; }
    public bool BoolValue { get; set; }
    public decimal DecimalValue { get; set; }
    public float FloatValue { get; set; }
    public double DoubleValue { get; set; }

    // High priority additions
    public DateTime DateTimeValue { get; set; }
    public DateTimeOffset DateTimeOffsetValue { get; set; }
    public Guid GuidValue { get; set; }
    public long LongValue { get; set; }
    public short ShortValue { get; set; }
    public byte ByteValue { get; set; }

    // Medium priority additions
    public DateOnly DateOnlyValue { get; set; }
    public TimeOnly TimeOnlyValue { get; set; }
    public TimeSpan TimeSpanValue { get; set; }
}
