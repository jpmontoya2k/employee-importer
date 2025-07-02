using CommandLine;

namespace EmployeeImporter.Cli;

public class ConversionOptions
{
    [Option('i', "input-file", Required = true, HelpText = "The full path to the CSV file to be processed.")]
    public string InputFilePath { get; set; }

    [Option('p', "parser", Required = true,
        HelpText = "The type of parser to use (e.g., 'TypeA', 'TypeB').")]
    public string ParserType { get; set; }
}