namespace EmployeeImporter.Cli.Common;

public interface IConvertingPipeline
{
    IAsyncEnumerable<RecordConversionResult> Run(TextReader inputReader);
}