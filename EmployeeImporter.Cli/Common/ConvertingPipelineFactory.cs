using EmployeeImporter.Cli.TypeAPipeline;

namespace EmployeeImporter.Cli.Common;

public class ConvertingPipelineFactory : IConvertingPipelineFactory
{
    public IConvertingPipeline Create(string inputFilePath, string parserType)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
            throw new ArgumentException("Input file path cannot be null or empty", nameof(inputFilePath));

        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException($"File not found: {inputFilePath}", inputFilePath);

        var reader = new StreamReader(File.OpenRead(inputFilePath));

        return parserType.ToLowerInvariant() switch
        {
            "typea" => new TypeAConvertingPipeline(reader),
            _ => throw new ArgumentException($"Unsupported parser type: {parserType}", nameof(parserType))
        };
    }
} 