using System.Text;

namespace funcscript.host;

public class StringLogger : FsLogger
{
    private StringBuilder _stringBuilder;

    public StringLogger()
    {
        _stringBuilder = new StringBuilder();
    }

    public override void WriteLine(string text)
    {
        _stringBuilder.AppendLine(text);
    }

    public override void Clear()
    {
        _stringBuilder.Clear();
    }

    public string GetLogContent()
    {
        return _stringBuilder.ToString();
    }
}
