using FuncScript.Core;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem.Exec.Funcs;

internal class InputFunction(ExecutionSession session) : IFsFunction
{
    public CallType CallType => CallType.Prefix;
    public string Symbol => "input";

    public object EvaluateList(KeyValueCollection context, FsList pars)
    {

        int? timeoutMs = null;
        if (pars.Length > 1)
            return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol}: 0 or 1 parameter expected, got {pars.Length}");

        if (pars.Length == 1)
        {
            if (pars[0] is not (double or int))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol}: An integer or double timeout value is expected.");

            timeoutMs = Convert.ToInt32(pars[0]);
        }

        try
        {
            return session.WaitForInput(timeoutMs);
        }
        catch (TimeoutException ex)
        {
            return new FsError(FsError.ERROR_TYPE_EVALUATION, ex.Message);
        }
    }
}