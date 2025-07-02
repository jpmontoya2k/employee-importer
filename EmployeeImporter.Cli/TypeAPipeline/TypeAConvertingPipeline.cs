using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.TypeAPipeline;

public class TypeAConvertingPipeline : IConvertingPipeline
{
    private readonly TextReader _inputReader;
    private bool _disposed;

    public TypeAConvertingPipeline(TextReader inputReader)
    {
        _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
    }

    public async IAsyncEnumerable<RecordConversionResult> ProcessRecords()
    {
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using var csvReader = new CsvReader(_inputReader, csvReaderConfig);

        foreach (var recordDto in csvReader.GetRecords<TypeARecordDto>())
        {
            var validationResult = TypeARecordDtoValidator.Instance.Validate(recordDto);
            if (validationResult.IsValid)
            {
                var result = recordDto.ToCommonModelDto();
                yield return RecordConversionResult.Success(result);
            }
            else
            {
                var rawData = csvReader.Parser.RawRecord;
                yield return RecordConversionResult.Failure(rawData, validationResult.Errors);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _inputReader?.Dispose();
            _disposed = true;
        }
    }
}