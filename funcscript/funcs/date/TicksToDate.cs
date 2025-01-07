using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class TicksToDateFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "TicksToDate";

        public object EvaluateList(FsList pars)
        {
            const int MaxParameters = 1;

            if (pars.Length > MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected a maximum of {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return null;

            if (!(par0 is long))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: Type mismatch. Expected a long.");

            var ticks = (long)par0;

            return new DateTime(ticks);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Ticks",
                _ => ""
            };
        }
    }
}
