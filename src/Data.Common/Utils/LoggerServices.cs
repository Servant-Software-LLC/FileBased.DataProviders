using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Data.Common.Utils;

public class LoggerServices
{
    private readonly Lazy<IServiceProvider> serviceProviderLazy;
    private readonly string logFilePath;
    private readonly LogLevel minimumLogLevel;

    public LoggerServices(LogLevel minimumLogLevel)
    {
        var executableName = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", string.Empty);
        logFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{executableName}");
        this.minimumLogLevel = minimumLogLevel;
        serviceProviderLazy = new Lazy<IServiceProvider>(InitializeServices);
    }


    public ILogger<T> CreateLogger<T>()
    {
        if (minimumLogLevel == LogLevel.None) 
            return new DummyLogger<T>();

        return serviceProviderLazy.Value.GetService<ILogger<T>>();
    }

    private IServiceProvider InitializeServices()
    {
        var newLogFilePath = $"{logFilePath}.log";
        int counter = 0;
        while (File.Exists(newLogFilePath))
        {
            counter++;
            newLogFilePath = $"{logFilePath}.{counter}.log";
        }

        // Initialize Serilog logger
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(ConvertToSerilogLevel(minimumLogLevel))
            .WriteTo.File(newLogFilePath)
            .CreateLogger();

        // Create service collection and configure it
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // Create service provider
        return serviceCollection.BuildServiceProvider();
    }

    private static LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,

            //Although LogLevel.None is a value that Serilog has no equivalent for, this method shouldn't get called when None is specified.
            _ => throw new Exception($"Log level {logLevel} was unexpected."),
        };
    }


    private static void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddSingleton(LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(dispose: true);
        }));

        services.AddLogging();
    }

    private class DummyLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Do nothing
        }
    }
}
