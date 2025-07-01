using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EmployeeImporter.Cli.TypeAPipeline;
using FluentAssertions;

namespace Sandbox;

public class TypeAPipelineConcept
{

    
    [Fact]
    public void Run()
    {
        var testCsv = Resources1.type_a;
        testCsv.Should().NotBeNullOrEmpty("test data is expected there");

        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using (var reader = new StringReader(testCsv))
        using (var csvReader = new CsvReader(reader, csvReaderConfig))
        {
            var results = new List<RecordConversionResult>();
            foreach (var recordDto in csvReader.GetRecords<TypeARecordDto>())
            {
                var validationResult = TypeARecordDtoValidator.Instance.Validate(recordDto);

                if (validationResult.IsValid)
                {
                    var result = recordDto.ToCommonModelDto();
                    results.Add(RecordConversionResult.Success(result));
                }
                else
                {
                    results.Add(RecordConversionResult.Failure(validationResult.Errors));
                }
            }
        }
    }
}