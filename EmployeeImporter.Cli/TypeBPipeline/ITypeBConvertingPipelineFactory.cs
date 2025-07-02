using Microsoft.Extensions.Logging;

namespace EmployeeImporter.Cli.TypeBPipeline;

public interface ITypeBConvertingPipelineFactory
{
    TypeBConvertingPipeline Create(TextReader reader);
}

public class TypeBConvertingPipelineFactory(ILoggerFactory loggerFactory, ITypeBRecordDtoConverter converter)
    : ITypeBConvertingPipelineFactory
{
    private readonly ILoggerFactory _loggerFactory =
        loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    private readonly ITypeBRecordDtoConverter _converter =
        converter ?? throw new ArgumentNullException(nameof(converter));

    public TypeBConvertingPipeline Create(TextReader reader)
    {
        var logger = _loggerFactory.CreateLogger<TypeBConvertingPipeline>();
        return new TypeBConvertingPipeline(reader, _converter, logger);
    }
}