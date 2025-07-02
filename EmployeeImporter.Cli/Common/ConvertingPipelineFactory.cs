using EmployeeImporter.Cli.TypeAPipeline;
using EmployeeImporter.Cli.TypeBPipeline;

namespace EmployeeImporter.Cli.Common;

public class ConvertingPipelineFactory : IConvertingPipelineFactory
{
    private readonly ITypeAConvertingPipelineFactory _aTypeConvertingPipelineFactory;
    private readonly ITypeBConvertingPipelineFactory _bTypeConvertingPipelineFactory;

    public ConvertingPipelineFactory(
        ITypeAConvertingPipelineFactory typeAConvertingPipelineFactory,
        ITypeBConvertingPipelineFactory typeBConvertingPipelineFactory
    )
    {
        _aTypeConvertingPipelineFactory = typeAConvertingPipelineFactory ??
                                          throw new ArgumentNullException(nameof(typeAConvertingPipelineFactory));
        _bTypeConvertingPipelineFactory = typeBConvertingPipelineFactory ??
                                          throw new ArgumentNullException(nameof(typeBConvertingPipelineFactory));
    }

    public IConvertingPipeline Create(string inputFilePath, string parserType)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
            throw new ArgumentException("Input file path cannot be null or empty", nameof(inputFilePath));

        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException($"File not found: {inputFilePath}", inputFilePath);

        var reader = new StreamReader(File.OpenRead(inputFilePath));

        return parserType.ToLowerInvariant() switch
        {
            "typea" => _aTypeConvertingPipelineFactory.Create(reader),
            "typeb" => _bTypeConvertingPipelineFactory.Create(reader),
            _ => throw new ArgumentException($"Unsupported parser type: {parserType}", nameof(parserType))
        };
    }
}