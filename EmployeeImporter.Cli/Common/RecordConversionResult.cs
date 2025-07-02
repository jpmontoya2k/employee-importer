using FluentValidation.Results;

namespace EmployeeImporter.Cli.Common;

public readonly struct RecordConversionResult
{
    public RecordConversionResult(CommonModelDto data)
    {
        Data = data;
    }

    public RecordConversionResult(string rawData, List<ValidationFailure>? validationErrors)
    {
        RawData = rawData;
        ValidationErrors = validationErrors;
    }

    public RecordConversionResult(string rawData, Exception processingError)
    {
        RawData = rawData;
        ProcessingError = processingError;
    }

    public CommonModelDto? Data { get; }
    public string? RawData { get; }
    public List<ValidationFailure>? ValidationErrors { get; }
    public Exception ProcessingError { get; }
    public bool IsSuccess() => ValidationErrors == null;

    // Factory methods for convenience
    public static RecordConversionResult Success(CommonModelDto data) => new(data);
    public static RecordConversionResult Failure(string rawData, List<ValidationFailure>? errors) => new(rawData.Trim(), errors);
    public static RecordConversionResult Failure(string rawData, Exception ex) => new(rawData.Trim(), ex);
}