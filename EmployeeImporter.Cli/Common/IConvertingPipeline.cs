namespace EmployeeImporter.Cli.Common;

public interface IConvertingPipeline : IDisposable
{
    IAsyncEnumerable<RecordConversionResult> ProcessRecords();
}