using Microsoft.Extensions.Logging;

namespace EmployeeImporter.Cli.TypeAPipeline;

public interface ITypeAConvertingPipelineFactory
{
    TypeAConvertingPipeline Create(TextReader reader);
}

public class TypeAConvertingPipelineFactory(ILoggerFactory loggerFactory) : ITypeAConvertingPipelineFactory
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ??  throw new ArgumentNullException(nameof(loggerFactory));

    public TypeAConvertingPipeline Create(TextReader reader)
    {
        var logger = _loggerFactory.CreateLogger<TypeAConvertingPipeline>();
        return new TypeAConvertingPipeline(reader, logger);
    }
}