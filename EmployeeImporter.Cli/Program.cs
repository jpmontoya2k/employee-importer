using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EmployeeImporter.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            using var host = CreateHost(args)
                .Build();
        
            var greeter = host.Services.GetRequiredService<WorldGreeter>();
        
            greeter.Greet();
                
            return 0;
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Fatal error");
            return 1;
        }
    }

    public static IHostBuilder CreateHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, lc) => lc.WriteTo.Console(), preserveStaticLogger: true)
            .ConfigureAppConfiguration(config =>
            {
                config
                    .AddJsonFile("appsettings.json", true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureServices(s => s.AddTransient<WorldGreeter>());
    }
}

public class WorldGreeter(ILogger<WorldGreeter> logger)
{
    private readonly ILogger<WorldGreeter> _logger = logger ??  throw new ArgumentNullException(nameof(logger));

    public void Greet()
    {
        _logger.LogInformation("Hello from Greeter");
    }
}