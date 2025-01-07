namespace FuncScript.Host;

public class ConsoleLogger : FsLogger
{
    public override void WriteLine(string text) => Console.WriteLine(text);
    public override void Clear() => Console.Clear();
}
public abstract class FsLogger
{
    public abstract void WriteLine(string text);
    public abstract void Clear();
        
    private static FsLogger _fsLogger;

    public static void SetDefaultLogger(FsLogger logger)
    {
        _fsLogger = logger;
    }
    public static FsLogger DefaultLogger => _fsLogger;

    static FsLogger()
    {
        SetDefaultLogger(new ConsoleLogger());
    }
}
