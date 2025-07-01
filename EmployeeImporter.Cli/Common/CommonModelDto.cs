namespace EmployeeImporter.Cli.Common;

/// <summary>
/// Represents the unified, canonical data record after parsing and transformation.
/// This is the common format that all source CSVs are mapped to.
/// </summary>
public class CommonModelDto
{
    /// <summary>
    /// The unique identifier for the employee or customer.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The full name of the person.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// The primary contact email address.
    /// </summary>
    public required string PrimaryEmail { get; set; }

    /// <summary>
    /// The primary contact phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The annual salary.
    /// </summary>
    public decimal? Salary { get; set; }
}