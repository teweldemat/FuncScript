namespace FuncScript.Model;

public class CodeLocation
{
    public int Loc { get; set; }
    public int Length { get; set; }
    public override string ToString()
    {
        return $"{Loc}:{Length}";
    }
}
public class FsError
{
    public static string CONTROL_BREAK="BREAK";
    public const string ERROR_DEFAULT = "Default";
    public const string ERROR_PARAMETER_COUNT_MISMATCH = "TOO_FEW_PARAMETER";
    public const string ERROR_TYPE_INVALID_PARAMETER = "TYPE_INVALID_PARAMETER";
    public const string ERROR_TYPE_EVALUATION = "TYPE_TYPE_EVALUATION";
    
    public string ErrorType { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorData { get; set; }
    private const int MaxMessageLength = 5000;
    public FsError(Exception ex)
    {
        var msg = ex.Message + "\n" + ex.StackTrace;
        var inner = ex.InnerException;
        while (inner != null)
        {
            msg += "\n" + ex.Message + "\n" + ex.StackTrace.Substring(0,MaxMessageLength);
            if(msg.Length>MaxMessageLength)
                break;
            inner = inner.InnerException;
        }

        ErrorType = ERROR_TYPE_EVALUATION;
        ErrorMessage = msg;
    }
    public FsError(string messsage) : this(ERROR_DEFAULT, messsage, null)
    {
        
    }
    public FsError(string type, string messsage) : this(type, messsage, null)
    {
        
    }

    public FsError(string type, string message, object data)
    {
        this.ErrorType = type;
        this.ErrorMessage = message;
        this.ErrorData = data;
    }

    public override string ToString()
    {
        return $"{this.ErrorMessage} ({this.ErrorType}){(ErrorData==null?"":"\n"+ErrorData.ToString())}";
    }
}
