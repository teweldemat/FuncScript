using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class GuidFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "guid";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected 1, but got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return null;

            if (!(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: Type mismatch. Expected a string.");

            var str = (string)par0;

            if (!Guid.TryParse(str, out var guid))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: String '{par0}' is not a valid GUID.");

            return guid;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Guid string",
                _ => ""
            };
        }
    }
}
