using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text;

public class UppercaseFunction : IFsFunction
{
    public CallType CallType => CallType.Prefix;
    public string Symbol => "uppercase";

    public object EvaluateList(FsList pars)
    {
        if (pars.Length == 0)
            throw new Error.EvaluationTimeException($"{this.Symbol} requires at least one parameter.");

        var input = pars[0] as string;
        if (input == null)
            return null;

        return input.ToUpper();
    }

    public string ParName(int index)
    {
        return index == 0 ? "string" : "";
    }
}
