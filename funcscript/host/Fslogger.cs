namespace funcscript.host;

public class ConsoleLogger : Fslogger
{
    public override void WriteLine(string text) => Console.WriteLine(text);
    public override void Clear() => Console.Clear();
}
public abstract class Fslogger
{
    public abstract void WriteLine(string text);
    public abstract void Clear();
        
    private static Fslogger _fslogger;

    public static void SetDefaultLogger(Fslogger logger)
    {
        _fslogger = logger;
    }
    public static Fslogger DefaultLogger => _fslogger;

    static Fslogger()
    {
        SetDefaultLogger(new ConsoleLogger());
    }
}