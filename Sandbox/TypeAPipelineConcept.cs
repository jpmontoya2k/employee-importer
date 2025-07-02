using EmployeeImporter.Cli.Common;
using EmployeeImporter.Cli.TypeAPipeline;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

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

        using var pipeline = new TypeAConvertingPipeline(reader, NullLogger<TypeAConvertingPipeline>.Instance);

        var results = new List<RecordConversionResult>();
        await foreach (var result in pipeline.ProcessRecords())
        {
            results.Add(result);
        }

        results.Count.Should().Be(7);

        // Verify successful conversions
        var successfulResults = results.Where(x => x.IsSuccess()).ToList();
        successfulResults.Count.Should().Be(4);

        // Check first successful record (Jane Doe)
        var janeResult = successfulResults.First();
        janeResult.Data.Should().NotBeNull();
        janeResult.Data!.Id.Should().Be("101");
        janeResult.Data.FullName.Should().Be("Jane Doe");
        janeResult.Data.PrimaryEmail.Should().Be("jane.doe@example.com");
        janeResult.Data.PhoneNumber.Should().Be("555-0101");
        janeResult.Data.Salary.Should().Be(75000.50m);

        // Check second successful record (John Smith)
        var johnResult = successfulResults[1];
        johnResult.Data!.Id.Should().Be("102");
        johnResult.Data.FullName.Should().Be("John Smith"); // Whitespace should be trimmed
        johnResult.Data.PrimaryEmail.Should().Be("john.smith@workplace.com"); // Whitespace should be trimmed
        johnResult.Data.PhoneNumber.Should().Be("555-0102");
        johnResult.Data.Salary.Should().Be(92000.00m);

        // Check third successful record (Bob Johnson)
        var bobResult = successfulResults[2];
        bobResult.Data!.Id.Should().Be("104");
        bobResult.Data.FullName.Should().Be("Bob Johnson");
        bobResult.Data.PrimaryEmail.Should().Be("bob.j@personal.co.uk");
        bobResult.Data.PhoneNumber.Should().Be("555-0104");
        bobResult.Data.Salary.Should().Be(110000m);

        // Check fourth successful record (Eve Williams)
        var eveResult = successfulResults[3];
        eveResult.Data!.Id.Should().Be("105");
        eveResult.Data.FullName.Should().Be("Eve Williams");
        eveResult.Data.PrimaryEmail.Should().Be("eve.williams@example.com");
        eveResult.Data.PhoneNumber.Should().BeNull(); // No phone number provided
        eveResult.Data.Salary.Should().Be(54000m);

        // Verify failed conversions
        var failedResults = results.Where(x => !x.IsSuccess()).ToList();
        failedResults.Count.Should().Be(3);
    }
}