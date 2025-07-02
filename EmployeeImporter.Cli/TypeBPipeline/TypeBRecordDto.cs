using CsvHelper.Configuration.Attributes;

namespace EmployeeImporter.Cli.TypeBPipeline;

/// <summary>
/// Represents a raw record from a Type-B CSV file.
/// This format has separate name fields and multiple email options.
/// It lacks a phone number, which must be sourced externally.
/// </summary>
public class TypeBRecordDto
{
    [Index(0)] public string ID { get; set; }

    [Index(1)] public string Name { get; set; }

    [Index(2)] public string Surname { get; set; }

    [Index(3)] public string? CorporateEmail { get; set; }

    // Should be prioritized if available.
    [Index(4)] public string? PersonalEmail { get; set; }

    // Represented as a string to handle potential currency symbols or formatting.
    [Index(5)] public string Salary { get; set; }

    [Ignore] public string Currency { get; set; }
}