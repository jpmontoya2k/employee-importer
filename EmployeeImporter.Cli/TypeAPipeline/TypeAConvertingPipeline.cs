using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EmployeeImporter.Cli.Common;
using Microsoft.Extensions.Logging;

namespace EmployeeImporter.Cli.TypeAPipeline;

public class TypeAConvertingPipeline : IConvertingPipeline
{
    private readonly TextReader _inputReader;
    private readonly ILogger<TypeAConvertingPipeline> _logger;
    private bool _disposed;

    public TypeAConvertingPipeline(TextReader inputReader, ILogger<TypeAConvertingPipeline> logger)
    {
        _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            yield return ProcessRecord(recordDto, csvReader);
        }
    }

    private RecordConversionResult ProcessRecord(TypeARecordDto recordDto, CsvReader csvReader)
    {
        var rawData = csvReader.Parser.RawRecord;
        try
        {
            var validationResult = TypeARecordDtoValidator.Instance.Validate(recordDto);
            if (!validationResult.IsValid)
            {
                return RecordConversionResult.Failure(rawData, validationResult.Errors);
            }

            var result = recordDto.ToCommonModelDto();
            return RecordConversionResult.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing record {Record}", csvReader.Parser.RawRecord);
            return RecordConversionResult.Failure(rawData, e);
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