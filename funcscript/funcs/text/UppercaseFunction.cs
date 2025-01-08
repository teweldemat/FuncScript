using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text;

public class UppercaseFunction : IFsFunction
{
    public CallType CallType => CallType.Prefix;
    public string Symbol => "uppercase";

    public object EvaluateList(KeyValueCollection context, FsList pars)
    {
        if (pars.Length == 0)
            return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: requires at least one parameter.");

        var input = pars[0] as string;
        if (input == null)
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: string expected");

        return input.ToUpper();
    }

    public string ParName(int index)
    {
        return index == 0 ? "string" : "";
    }
}
