using System.Globalization;
using EmployeeImporter.Cli.Common;
using Microsoft.Extensions.Logging;

namespace EmployeeImporter.Cli.TypeBPipeline;

/// <summary>
/// Converts TypeBRecordDto to CommonModelDto.
/// Assumes the input record has passed validation.
/// </summary>
public class TypeBRecordDtoConverter(ICurrencyConverter currencyConverter, ILogger<TypeBRecordDtoConverter> logger)
    : ITypeBRecordDtoConverter
{
    private readonly ICurrencyConverter _currencyConverter =
        currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));

    private readonly ILogger<TypeBRecordDtoConverter> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Maps a TypeBRecordDto to CommonModelDto.
    /// </summary>
    /// <param name="source">The source TypeBRecordDto to map from.</param>
    /// <param name="targetCurrency">Currency to convert salary to</param>
    /// <returns>A new CommonModelDto instance with mapped values.</returns>
    public async Task<CommonModelDto> ToCommonModelDto(TypeBRecordDto source, string targetCurrency)
    {
        var salary = await ProcessSalary(source, targetCurrency);

        return new CommonModelDto
        {
            Id = source.ID!.Trim(),
            FullName = $"{source.Name!.Trim()} {source.Surname!.Trim()}".Trim(),
            PrimaryEmail = GetPrimaryEmail(source),
            PhoneNumber = null, // TypeB format doesn't include phone numbers
            Salary = salary
        };
    }

    private async Task<decimal> ProcessSalary(TypeBRecordDto source, string targetCurrency)
    {
        var salary = decimal.Parse(source.Salary!, NumberStyles.Any, CultureInfo.InvariantCulture);

        if (source.Currency != null && source.Currency != targetCurrency)
        {
            try
            {
                salary = await _currencyConverter.ConvertCurrency(salary, source.Currency, targetCurrency);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error converting currency by API for salary {Salary} from {SourceCurrency} to {TargetCurrency}", salary, source.Currency, targetCurrency);
                throw;
            }
            
        }

        return salary;
    }

    /// <summary>
    /// Determines the primary email address, prioritizing PersonalEmail over CorporateEmail.
    /// </summary>
    /// <param name="source">The source TypeBRecordDto.</param>
    /// <returns>The primary email address.</returns>
    private static string GetPrimaryEmail(TypeBRecordDto source)
    {
        // Prioritize PersonalEmail as noted in the TypeBRecordDto comments
        return !string.IsNullOrWhiteSpace(source.PersonalEmail)
            ? source.PersonalEmail!.Trim()
            : source.CorporateEmail!.Trim();
    }
}

public interface ITypeBRecordDtoConverter
{
    Task<CommonModelDto> ToCommonModelDto(TypeBRecordDto source, string targetCurrency);
}