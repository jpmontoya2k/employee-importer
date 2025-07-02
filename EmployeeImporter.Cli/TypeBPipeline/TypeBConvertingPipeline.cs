using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EmployeeImporter.Cli.Common;
using Microsoft.Extensions.Logging;

namespace EmployeeImporter.Cli.TypeBPipeline;

public class TypeBConvertingPipeline : IConvertingPipeline
{
    private const string DefaultCurrency = "EUR";

    private readonly TextReader _inputReader;
    private readonly ITypeBRecordDtoConverter _converter;
    private readonly ILogger<TypeBConvertingPipeline> _logger;
    private bool _disposed;

    public TypeBConvertingPipeline(TextReader inputReader,
        ITypeBRecordDtoConverter converter,
        ILogger<TypeBConvertingPipeline> logger)
    {
        _inputReader = inputReader ?? throw new ArgumentNullException(nameof(inputReader));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async IAsyncEnumerable<RecordConversionResult> ProcessRecords()
    {
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using var csvReader = new CsvReader(_inputReader, csvReaderConfig);

        foreach (var recordDto in csvReader.GetRecords<TypeBRecordDto>())
        {
            yield return await ProcessRecord(recordDto, csvReader);
        }
    }

    private async Task<RecordConversionResult> ProcessRecord(TypeBRecordDto recordDto, CsvReader csvReader)
    {
        var rawData = csvReader.Parser.RawRecord;

        try
        {
            var validationResult = TypeBRecordDtoValidator.Instance.Validate(recordDto);
            if (!validationResult.IsValid)
            {
                return RecordConversionResult.Failure(rawData, validationResult.Errors);
            }

            var (salary, currency) = SalaryParser.ParseSalaryAndCurrency(recordDto.Salary!);
            recordDto.Salary = salary;
            recordDto.Currency = currency ?? DefaultCurrency;

            var result = await _converter.ToCommonModelDto(recordDto, DefaultCurrency);
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