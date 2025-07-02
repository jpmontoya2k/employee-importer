namespace EmployeeImporter.Cli.Common;

public interface IConvertingPipelineFactory
{
    IConvertingPipeline Create(string inputFilePath, string parserType);
} 