using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class OrFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "or";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];

                if (!(par is bool b))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"{this.Symbol} doesn't apply to this type:{(par == null ? "null" : par.GetType().ToString())}");

                if (b)
                    return true;
            }

            return false;
        }

        public string ParName(int index)
        {
            return $"Value {index + 1}";
        }
    }
}
