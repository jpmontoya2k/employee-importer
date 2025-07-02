using EmployeeImporter.Cli.Common;
using EmployeeImporter.Cli.Output;

namespace EmployeeImporter.Cli;

public interface IRecordsImporter
{
    Task ImportRecords(ConversionOptions options);
}

public class RecordsImporter : IRecordsImporter
{
    private readonly IConvertingPipelineFactory _pipelineFactory;
    private readonly IResultsWriterFactory _resultsWriterFactory;

    public RecordsImporter(IConvertingPipelineFactory pipelineFactory, IResultsWriterFactory resultsWriterFactory)
    {
        _pipelineFactory = pipelineFactory ?? throw new ArgumentNullException(nameof(pipelineFactory));
        _resultsWriterFactory = resultsWriterFactory ?? throw new ArgumentNullException(nameof(resultsWriterFactory));
    }

    public async Task ImportRecords(ConversionOptions options)
    {
        using var pipeline = _pipelineFactory.Create(options.InputFilePath, options.ParserType);
        using var resultsWriter = _resultsWriterFactory.Create(options.InputFilePath);

        await foreach (var result in pipeline.ProcessRecords())
        {
            resultsWriter.Persist(result);
        }
    }
}