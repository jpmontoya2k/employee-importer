using System.Text.RegularExpressions;
using FluentValidation;

namespace EmployeeImporter.Cli.TypeAPipeline;

public partial class TypeARecordDtoValidator : AbstractValidator<TypeARecordDto>
{
    public static readonly IValidator<TypeARecordDto> Instance = new TypeARecordDtoValidator();

    private TypeARecordDtoValidator()
    {
        RuleFor(x => x.CustomerID)
            .NotEmpty().WithMessage("Customer ID is required.")
            .Must(id => int.TryParse(id, out _)).WithMessage("Customer ID must be a valid number.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .Must(BeAValidFullName).WithMessage("Full Name must contain at least a first and last name separated by a space.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .Must(email => email!.Contains('@'))
            .WithMessage("A valid email address must contain an '@' symbol.");

        RuleFor(x => x.Phone)
            .ChildRules(phone => phone.RuleFor(p => p)
                .Must(HaveValidPhoneCharacters).WithMessage("Phone number contains invalid characters. Only numbers, spaces, parentheses, hyphens, and a leading plus are allowed.")
            )
            .When(x => !string.IsNullOrWhiteSpace(x.Phone), ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Salary is required.")
            .Must(salary => decimal.TryParse(salary, out var parsedValue) && parsedValue > 0)
            .WithMessage("Salary must be a valid positive number.");
    }

    /// <summary>
    /// Custom validator to check if the FullName string contains at least two parts.
    /// </summary>
    private static bool BeAValidFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return false;
        }

        var parts = fullName.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        // Check if we are left with at least two parts (e.g., "First" and "Last").
        return parts.Length >= 2;
    }
    
    /// <summary>
    /// Custom validator for phone number characters.
    /// A good candidate for an external API call for some more sophisticated phone validation API.
    /// </summary>
    private static bool HaveValidPhoneCharacters(string? phone)
    {
        return string.IsNullOrWhiteSpace(phone) ||
               MyRegex().IsMatch(phone.Trim());
    }

    [GeneratedRegex(@"^\+?[\d\s\(\)\-]+$")]
    private static partial Regex MyRegex();
}