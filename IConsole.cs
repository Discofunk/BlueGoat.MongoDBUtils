namespace BlueGoat.MongoDBUtils;

public interface IConsole
{
    void WriteLine(string? message);
    void WriteLineOk(string? message);
    void WriteLineInfo(string? message);
    void WriteLineError(string? message);
    void WriteLineWarn(string? message);
    void Write(string? message);
    void WriteOk(string? message);
    void WriteInfo(string? message);
    void WriteError(string? message);
    void WriteWarn(string? message);
}