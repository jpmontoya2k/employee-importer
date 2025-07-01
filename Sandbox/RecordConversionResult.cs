using EmployeeImporter.Cli.Common;
using FluentValidation.Results;

namespace Sandbox;

public readonly struct RecordConversionResult
{
    public RecordConversionResult(CommonModelDto data)
    {
        Data = data;
    }

    public RecordConversionResult(List<ValidationFailure>? errors)
    {
        Errors = errors;
    }

    public CommonModelDto? Data { get; }
    public List<ValidationFailure>? Errors { get; }
    public bool IsSuccess => Errors == null;

    // Factory methods for convenience
    public static RecordConversionResult Success(CommonModelDto data) => new(data);
    public static RecordConversionResult Failure(List<ValidationFailure>? errors) => new(errors);
}