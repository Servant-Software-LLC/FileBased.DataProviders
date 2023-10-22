using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Json.Infrastructure.Internal;

/// <summary>
/// Represents the JSON-specific implementation of relational options used for Entity Framework Core configuration.
/// This extension is used to enable and configure the usage of JSON as a data source.
/// </summary>
public class JsonOptionsExtension : RelationalOptionsExtension
{
    // Cached instance of the associated options extension info
    private JsonOptionsExtensionInfo _info;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonOptionsExtension"/> class.
    /// </summary>
    public JsonOptionsExtension() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonOptionsExtension"/> class, copying settings from the provided instance.
    /// </summary>
    /// <param name="copyFrom">The instance to copy settings from.</param>
    protected internal JsonOptionsExtension(JsonOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    /// <summary>
    /// Gets the information associated with this options extension.
    /// </summary>
    public override DbContextOptionsExtensionInfo Info => _info ??= new JsonOptionsExtensionInfo(this);

    /// <summary>
    /// Configures the dependency injection services for the JSON provider.
    /// </summary>
    /// <param name="services">The collection of services to add to.</param>
    public override void ApplyServices(IServiceCollection services)
    {
        services.AddEntityFrameworkJson();
    }


    /// <summary>
    /// Validates the configuration for the JSON data source.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    public override void Validate(IDbContextOptions options)
    {
        // Implementation for any specific validation logic for the JSON provider.
    }

    /// <summary>
    /// Creates a copy of this options extension.
    /// </summary>
    /// <returns>A cloned instance of this options extension.</returns>
    protected override RelationalOptionsExtension Clone() => new JsonOptionsExtension(this);
}
