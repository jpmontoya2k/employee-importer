using EmployeeImporter.Cli.Common;
using EmployeeImporter.Cli.TypeAPipeline;
using FluentAssertions;

namespace Sandbox;

public class TypeAPipelineConcept
{
    [Fact]
    public async Task Run()
    {
        const string testCsv = """
                               CustomerID,FullName,Email,Phone,Salary
                               101,Jane Doe,jane.doe@example.com,555-0101,75000.50
                               102, John Smith , john.smith@workplace.com ,555-0102,92000.00
                               103,Alice,invalid.email,555-0103,68000.75
                               104,Bob Johnson,bob.j@personal.co.uk,555-0104,110000
                               105,Eve Williams,eve.williams@example.com,,54000
                               106,Kate Ho,kate@corp.com,invalid-phone,55000
                               107,Don Ho,kate@corp.com,invalid-phone,-55000
                               """;

        var reader = new StringReader(testCsv);

        var pipeline = new TypeAConvertingPipeline();

        var results = new List<RecordConversionResult>();
        await foreach (var result in pipeline.Run(reader))
        {
            results.Add(result);
        }

        results.Count.Should().Be(7);
        results.Count(x => x.IsSuccess()).Should().Be(4);
        results.Count(x => !x.IsSuccess()).Should().Be(3);
    }
}