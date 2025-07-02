using FluentValidation.Results;

namespace EmployeeImporter.Cli.Common;

public readonly struct RecordConversionResult
{
    public RecordConversionResult(CommonModelDto data)
    {
        Data = data;
    }

    public RecordConversionResult(string rawData, List<ValidationFailure>? errors)
    {
        RawData = rawData;
        Errors = errors;
    }

    public CommonModelDto? Data { get; }
    public string? RawData { get; }
    public List<ValidationFailure>? Errors { get; }
    public bool IsSuccess() => Errors == null;

    // Factory methods for convenience
    public static RecordConversionResult Success(CommonModelDto data) => new(data);
    public static RecordConversionResult Failure(string rawData, List<ValidationFailure>? errors) => new(rawData.Trim(), errors);
}