using EmployeeImporter.Cli.TypeAPipeline;
using EmployeeImporter.Cli.TypeBPipeline;

namespace EmployeeImporter.Cli.Common;

public class ConvertingPipelineFactory : IConvertingPipelineFactory
{
    private readonly IFileSystem _fileSystem;
    private readonly ITypeAConvertingPipelineFactory _aTypeConvertingPipelineFactory;
    private readonly ITypeBConvertingPipelineFactory _bTypeConvertingPipelineFactory;

    public ConvertingPipelineFactory(
        ITypeAConvertingPipelineFactory typeAConvertingPipelineFactory,
        ITypeBConvertingPipelineFactory typeBConvertingPipelineFactory, IFileSystem fileSystem)
    {
        _aTypeConvertingPipelineFactory = typeAConvertingPipelineFactory ??
                                          throw new ArgumentNullException(nameof(typeAConvertingPipelineFactory));
        _bTypeConvertingPipelineFactory = typeBConvertingPipelineFactory ??
                                          throw new ArgumentNullException(nameof(typeBConvertingPipelineFactory));
        _fileSystem = fileSystem ??  throw new ArgumentNullException(nameof(fileSystem));
    }

    public IConvertingPipeline Create(string inputFilePath, string parserType)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
            throw new ArgumentException("Input file path cannot be null or empty", nameof(inputFilePath));

        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException($"File not found: {inputFilePath}", inputFilePath);

        var reader = _fileSystem.OpenFileReader(inputFilePath);

        return parserType.ToLowerInvariant() switch
        {
            "typea" => _aTypeConvertingPipelineFactory.Create(reader),
            "typeb" => _bTypeConvertingPipelineFactory.Create(reader),
            _ => throw new ArgumentException($"Unsupported parser type: {parserType}", nameof(parserType))
        };
    }
}