using CsvHelper.Configuration.Attributes;

namespace EmployeeImporter.Cli.TypeAPipeline;

/// <summary>
/// Represents a raw record from a Type-A CSV file.
/// Uses Index attributes for mapping with CsvHelper.
/// All properties are strings to capture the raw data before type conversion and validation.
/// </summary>
public class TypeARecordDto
{
    [Index(0)] public string? CustomerID { get; set; }
    [Index(1)] public string? FullName { get; set; }
    [Index(2)] public string? Email { get; set; }
    [Index(3)] public string? Phone { get; set; }
    [Index(4)] public string? Salary { get; set; }
}