using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.List
{
    public class LengthFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Len";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameters = 1;
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];

            return EvaluateInternal(par0);
        }

        private object EvaluateInternal(object par0)
        {
            return par0 switch
            {
                null => 0,
                FsList list => list.Length,
                string s => s.Length,
                _ => new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function doesn't apply to {par0.GetType()}")
            };
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List or String",
                _ => ""
            };
        }
    }
}
