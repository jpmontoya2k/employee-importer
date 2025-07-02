using CommandLine;
using EmployeeImporter.Cli.Output;
using EmployeeImporter.Cli.TypeAPipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ILogger = Serilog.ILogger;

namespace EmployeeImporter.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var exitCode = Parser.Default.ParseArguments<ConversionOptions>(args)
            .MapResult(
                opts => RunApplication(args, opts, Log.Logger).Result,
                _ => 1
            );

        Log.CloseAndFlush();

        return exitCode;
    }

    public static async Task<int> RunApplication(string[] args, ConversionOptions options, ILogger logger)
    {
        try
        {
            if (!File.Exists(options.InputFilePath))
            {
                logger.Fatal("File {ValueFilePath} does not exist", options.InputFilePath);
                return 1;
            }

            using var host = CreateHost(args)
                .Build();

            var pipeline = host.Services.GetRequiredService<TypeAConvertingPipeline>();
            using var reader = new StreamReader(File.OpenRead(options.InputFilePath));
            using var resultsWriter = ResultsWriterFactory.Create(options.InputFilePath);
            
            await foreach (var result in pipeline.Run(reader))
            {
                resultsWriter.Persist(result);
            }

            return 0;
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Fatal error");
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
            .ConfigureServices(s => s.AddTransient<TypeAConvertingPipeline>());
    }
}