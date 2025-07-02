using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.Output;

public interface IResultsWriter : IDisposable
{
    void Persist(RecordConversionResult result);
} 

public class ResultsWriter : IResultsWriter
{
    private readonly CsvWriter _csvWriter;
    private readonly StreamWriter _errorsWriter;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public ResultsWriter(CsvWriter csvWriter, StreamWriter errorsWriter)
    {
        _csvWriter = csvWriter ?? throw new ArgumentNullException(nameof(csvWriter));
        _errorsWriter = errorsWriter ?? throw new ArgumentNullException(nameof(errorsWriter));
        
        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public void Persist(RecordConversionResult result)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ResultsWriter));
        
        if (result.IsSuccess())
        {
            _csvWriter.WriteRecord(result.Data);
            _csvWriter.NextRecord();
        }
        else
        {
            var json = JsonSerializer.Serialize(result, _jsonOptions);
            _errorsWriter.WriteLine(json);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _csvWriter?.Dispose();
        _errorsWriter?.Dispose();
        _disposed = true;
    }
} 