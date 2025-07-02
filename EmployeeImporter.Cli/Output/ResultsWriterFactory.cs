using System.Globalization;
using CsvHelper;

namespace EmployeeImporter.Cli.Output;

public interface IResultsWriterFactory
{
    IResultsWriter Create(string inputFilePath);
}

public class ResultsWriterFactory : IResultsWriterFactory
{
    public IResultsWriter Create(string inputFilePath)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var directory = Path.GetDirectoryName(inputFilePath);
        var commonOutputPath = Path.Combine(directory ?? ".", $"{baseFileName}.common.csv");
        var errorsOutputPath = Path.Combine(directory ?? ".", $"{baseFileName}.errors.jsonl");

        var commonWriter = new StreamWriter(commonOutputPath);
        var csvWriter = new CsvWriter(commonWriter, CultureInfo.InvariantCulture);
        var errorsWriter = new StreamWriter(errorsOutputPath);

        return new ResultsWriter(csvWriter, errorsWriter);
    }
} 