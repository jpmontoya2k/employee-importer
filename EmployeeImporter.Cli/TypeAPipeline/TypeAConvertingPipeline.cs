using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.TypeAPipeline;

public class TypeAConvertingPipeline : IConvertingPipeline
{
    public async IAsyncEnumerable<RecordConversionResult> Run(TextReader inputReader)
    {
        // 
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using var csvReader = new CsvReader(inputReader, csvReaderConfig);

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
}