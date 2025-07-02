using CommandLine;
using EmployeeImporter.Cli.Common;
using EmployeeImporter.Cli.Output;
using EmployeeImporter.Cli.TypeAPipeline;
using EmployeeImporter.Cli.TypeBPipeline;
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
            using var host = CreateHost(args)
                .Build();

            var importer = host.Services.GetRequiredService<IRecordsImporter>();

            await importer.ImportRecords(options);

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
            .ConfigureServices(s =>
            {
                s.AddTransient<IRecordsImporter, RecordsImporter>();
                s.AddTransient<IFileSystem, OsFileSystem>();
                s.AddTransient<IConvertingPipelineFactory, ConvertingPipelineFactory>();
                s.AddTransient<IResultsWriterFactory, ResultsWriterFactory>();
                
                // Type A
                s.AddTransient<ITypeAConvertingPipelineFactory, TypeAConvertingPipelineFactory>();
                
                // Type B
                s.AddTransient<ITypeBConvertingPipelineFactory, TypeBConvertingPipelineFactory>();
                s.AddTransient<ITypeBRecordDtoConverter, TypeBRecordDtoConverter>();
                s.AddTransient<ICurrencyConverter, NoopCurrencyConverter>();
            });
    }
}