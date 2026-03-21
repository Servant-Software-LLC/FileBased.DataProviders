using EFCore.Common.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Common.Tests;

public static class TypeMappingTests<TContext> where TContext : TypeMappingContextBase, new()
{
    /// <summary>
    /// Verifies that EF Core can build a model with entities using all the expanded type mappings.
    /// This confirms the type mapping source correctly resolves CLR types to store types.
    /// </summary>
    public static void ModelBuilder_CanMapAllExtendedTypes(string connectionString)
    {
        using var db = new TContext();
        db.ConnectionString = connectionString;

        // Force model creation — this will throw if any type mapping is missing
        var model = db.Model;
        var entityType = model.FindEntityType(typeof(TypeMappingEntity));

        Assert.NotNull(entityType);

        // Verify all properties are recognized by the model
        var expectedProperties = new[]
        {
            nameof(TypeMappingEntity.Id),
            nameof(TypeMappingEntity.StringValue),
            nameof(TypeMappingEntity.IntValue),
            nameof(TypeMappingEntity.BoolValue),
            nameof(TypeMappingEntity.DecimalValue),
            nameof(TypeMappingEntity.FloatValue),
            nameof(TypeMappingEntity.DoubleValue),
            nameof(TypeMappingEntity.DateTimeValue),
            nameof(TypeMappingEntity.DateTimeOffsetValue),
            nameof(TypeMappingEntity.GuidValue),
            nameof(TypeMappingEntity.LongValue),
            nameof(TypeMappingEntity.ShortValue),
            nameof(TypeMappingEntity.ByteValue),
            nameof(TypeMappingEntity.DateOnlyValue),
            nameof(TypeMappingEntity.TimeOnlyValue),
            nameof(TypeMappingEntity.TimeSpanValue),
        };

        foreach (var propertyName in expectedProperties)
        {
            var property = entityType.FindProperty(propertyName);
            Assert.NotNull(property);

            // Verify that a relational type mapping has been resolved for this property
            var typeMapping = property.GetRelationalTypeMapping();
            Assert.NotNull(typeMapping);
            Assert.False(string.IsNullOrEmpty(typeMapping.StoreType),
                $"Property '{propertyName}' should have a non-empty store type, but got null/empty.");
        }
    }

    /// <summary>
    /// Verifies that EnsureCreated succeeds with the extended type entity,
    /// confirming the type mappings produce valid DDL.
    /// </summary>
    public static void EnsureCreated_WithExtendedTypes(string connectionString)
    {
        using var db = new TContext();
        db.ConnectionString = connectionString;
        db.Database.EnsureCreated();

        // If we reach here without exception, EnsureCreated succeeded
        // which means the type mappings produced valid table creation
        Assert.NotNull(db.TypeMappingEntities);
    }
}
