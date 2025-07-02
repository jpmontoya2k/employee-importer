namespace EmployeeImporter.Cli.Common;

public interface IFileSystem
{
    StreamReader OpenFileReader(string path);
    StreamWriter CreateFileWriter(string path);
}

public class OsFileSystem : IFileSystem
{
    public StreamReader OpenFileReader(string path)
    {
        return new StreamReader(File.OpenRead(path));
    }

    public StreamWriter CreateFileWriter(string path)
    {
        return new StreamWriter(path);
    }
}