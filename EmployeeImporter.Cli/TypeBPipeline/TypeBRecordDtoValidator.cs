using System.Globalization;
using FluentValidation;

namespace EmployeeImporter.Cli.TypeBPipeline;

public class TypeBRecordDtoValidator : AbstractValidator<TypeBRecordDto>
{
    public static readonly IValidator<TypeBRecordDto> Instance = new TypeBRecordDtoValidator();

    private TypeBRecordDtoValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.");

        RuleFor(x => x)
            .Must(dto =>
                !string.IsNullOrWhiteSpace(dto.CorporateEmail) || !string.IsNullOrWhiteSpace(dto.PersonalEmail))
            .WithMessage("At least one email (Corporate or Personal) must be provided.")
            .WithName("Email");

        RuleFor(x => x.CorporateEmail)
            .Must(email => email!.Contains('@'))
            .When(x => !string.IsNullOrWhiteSpace(x.CorporateEmail))
            .WithMessage("Corporate Email must contain an '@' symbol.");

        RuleFor(x => x.PersonalEmail)
            .Must(email => email!.Contains('@'))
            .When(x => !string.IsNullOrWhiteSpace(x.PersonalEmail))
            .WithMessage("Personal Email must contain an '@' symbol.");

        // The rule is adapted to handle potential currency symbols and thousands separators.
        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Salary is required.")
            .Must(BeAValidSalary)
            .WithMessage("Salary must be a valid positive number. Currency symbols and commas are permitted.");
    }

    private static bool BeAValidSalary(string? salary)
    {
        if (string.IsNullOrWhiteSpace(salary))
        {
            return false;
        }

        var skipSymbols = 0;
        if (salary.StartsWith('$') || salary.StartsWith('€'))
        {
            skipSymbols = 1;
        }
        else if (salary.StartsWith("EUR") || salary.StartsWith("USD"))
        {
            skipSymbols = 3;
        }

        if (salary.Length <= skipSymbols)
        {
            return false;
        }

        salary = salary[skipSymbols ..];

        var isParsable = decimal.TryParse(salary, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue);

        return isParsable && parsedValue > 0;
    }
}