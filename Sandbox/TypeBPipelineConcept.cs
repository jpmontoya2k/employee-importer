using EmployeeImporter.Cli.Common;
using EmployeeImporter.Cli.TypeBPipeline;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sandbox;

public class TypeBPipelineConcept
{
    [Fact]
    public async Task Run()
    {
        const string testCsv = """
                               ID,Name,Surname,CorporateEmail,PersonalEmail,Salary
                               USR-201,Carlos,Santana,c.santana@company.com,carlos.s@email.com,"$85,000"
                               USR-202,Diana,Ross,d.ross@company.com,,$150000.00
                               USR-203,Frank,Zappa,,frank.z@mail.net,99000
                               USR-204,Grace,Jones,g.jones@company.com,grace.jones@personal.org,"120,500.25"
                               USR-205,Henry,Rollins,,,"$65000"
                               USR-206,Invalid,User,invalid-email,,75000
                               USR-207,Missing,Name,,,50000
                               """;

        var reader = new StringReader(testCsv);

        var converter = new TypeBRecordDtoConverter(new NoopCurrencyConverter(), NullLogger<TypeBRecordDtoConverter>.Instance);
        using var pipeline = new TypeBConvertingPipeline(reader, converter, NullLogger<TypeBConvertingPipeline>.Instance);

        var results = new List<RecordConversionResult>();
        await foreach (var result in pipeline.ProcessRecords())
        {
            results.Add(result);
        }

        results.Count.Should().Be(7);

        // Verify successful conversions
        var successfulResults = results.Where(x => x.IsSuccess()).ToList();
        successfulResults.Count.Should().Be(4);

        // Check first successful record (Carlos Santana)
        var carlosResult = successfulResults.First();
        carlosResult.Data.Should().NotBeNull();
        carlosResult.Data!.Id.Should().Be("USR-201");
        carlosResult.Data.FullName.Should().Be("Carlos Santana");
        carlosResult.Data.PrimaryEmail.Should().Be("carlos.s@email.com"); // Personal email prioritized
        carlosResult.Data.PhoneNumber.Should().BeNull(); // TypeB doesn't have phone numbers
        carlosResult.Data.Salary.Should().Be(85000m);

        // Check second successful record (Diana Ross)
        var dianaResult = successfulResults[1];
        dianaResult.Data!.Id.Should().Be("USR-202");
        dianaResult.Data.FullName.Should().Be("Diana Ross");
        dianaResult.Data.PrimaryEmail.Should().Be("d.ross@company.com"); // Corporate email used (no personal)
        dianaResult.Data.Salary.Should().Be(150000m);

        // Check third successful record (Frank Zappa)
        var frankResult = successfulResults[2];
        frankResult.Data!.Id.Should().Be("USR-203");
        frankResult.Data.FullName.Should().Be("Frank Zappa");
        frankResult.Data.PrimaryEmail.Should().Be("frank.z@mail.net"); // Personal email used (no corporate)
        frankResult.Data.Salary.Should().Be(99000m);

        // Check fourth successful record (Grace Jones)
        var graceResult = successfulResults[3];
        graceResult.Data!.Id.Should().Be("USR-204");
        graceResult.Data.FullName.Should().Be("Grace Jones");
        graceResult.Data.PrimaryEmail.Should().Be("grace.jones@personal.org"); // Personal email prioritized
        graceResult.Data.Salary.Should().Be(120500.25m);

        // Verify failed conversions
        var failedResults = results.Where(x => !x.IsSuccess()).ToList();
        failedResults.Count.Should().Be(3);
    }
} 