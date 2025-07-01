using System.Globalization;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.TypeAPipeline;

/// <summary>
/// Maps TypeARecordDto to CommonModelDto.
/// Assumes the input record has passed validation.
/// </summary>
public static class TypeARecordDtoConverter
{
    /// <summary>
    /// Maps a TypeARecordDto to CommonModelDto.
    /// </summary>
    /// <param name="source">The source TypeARecordDto to map from.</param>
    /// <returns>A new CommonModelDto instance with mapped values.</returns>
    public static CommonModelDto ToCommonModelDto(this TypeARecordDto source)
    {
        return new CommonModelDto
        {
            Id = int.Parse(source.CustomerID!).ToString(),
            FullName = source.FullName!.Trim(),
            PrimaryEmail = source.Email!.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(source.Phone) ? null : source.Phone.Trim(),
            Salary = decimal.Parse(source.Salary!, NumberStyles.Any, CultureInfo.InvariantCulture)
        };
    }
} 