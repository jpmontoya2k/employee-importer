using System.Globalization;
using CsvHelper;
using EmployeeImporter.Cli.Common;

namespace EmployeeImporter.Cli.Output;

public interface IResultsWriterFactory
{
    IResultsWriter Create(string inputFilePath);
}

public class ResultsWriterFactory : IResultsWriterFactory
{
    private readonly IFileSystem _fileSystem;

    public ResultsWriterFactory(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public IResultsWriter Create(string inputFilePath)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var directory = Path.GetDirectoryName(inputFilePath);
        var commonOutputPath = Path.Combine(directory ?? ".", $"{baseFileName}.common.csv");
        var errorsOutputPath = Path.Combine(directory ?? ".", $"{baseFileName}.errors.jsonl");

        var commonWriter = _fileSystem.CreateFileWriter(commonOutputPath);
        var csvWriter = new CsvWriter(commonWriter, CultureInfo.InvariantCulture);
        var errorsWriter = _fileSystem.CreateFileWriter(errorsOutputPath);

        return new ResultsWriter(csvWriter, errorsWriter);
    }
}