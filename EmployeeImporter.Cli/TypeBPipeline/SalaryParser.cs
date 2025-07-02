using System.Globalization;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.TypeBPipeline;

/// <summary>
/// Maps TypeBRecordDto to CommonModelDto.
/// Assumes the input record has passed validation.
/// </summary>
public static class SalaryParser
{
    /// <summary>
    /// Parses salary and extracts currency symbol using the same logic as the validator.
    /// </summary>
    /// <param name="salaryString">The salary string to parse.</param>
    /// <returns>A tuple containing the parsed salary and currency symbol.</returns>
    public static (string salary, string? currency) ParseSalaryAndCurrency(string salaryString)
    {
        var skipSymbols = 0;
        string? currency = null;

        if (salaryString.StartsWith('$'))
        {
            skipSymbols = 1;
            currency = "$";
        }
        else if (salaryString.StartsWith('€'))
        {
            skipSymbols = 1;
            currency = "€";
        }
        else if (salaryString.StartsWith("EUR"))
        {
            skipSymbols = 3;
            currency = "EUR";
        }
        else if (salaryString.StartsWith("USD"))
        {
            skipSymbols = 3;
            currency = "USD";
        }

        if (salaryString.Length <= skipSymbols)
        {
            throw new ArgumentException("Invalid salary format", nameof(salaryString));
        }

        var numericPart = salaryString[skipSymbols..];
        return (numericPart, currency);
    }
} 