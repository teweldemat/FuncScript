using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class GuidFunction : IFsFunction
    {
        private const int MaxParametersCount = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "guid";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParametersCount)
                throw new error.EvaluationTimeException(
                    $"{this.Symbol} function: Invalid parameter count. Expected {MaxParametersCount}, but got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return null;

            if (!(par0 is string))
                throw new error.TypeMismatchError(
                    $"Function {this.Symbol}: Type mismatch. Expected a string.");

            var str = (string)par0;

            if (!Guid.TryParse(str, out var guid))
                throw new error.TypeMismatchError(
                    $"Function {this.Symbol}: String '{par0}' is not a valid GUID.");

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