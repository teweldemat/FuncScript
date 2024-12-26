using funcscript.core;

namespace funcscript.funcs.logic
{
    public class GuidFunction : IFsFunction
    {
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "guid";

        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                throw new error.EvaluationTimeException(
                    $"{this.Symbol} function: Invalid parameter count. Expected {this.MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);

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