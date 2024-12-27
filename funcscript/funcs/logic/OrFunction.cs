using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class OrFunction : IFsFunction
    {
        private const int MaxParametersCount = -1; // Replacing MaxParsCount property

        public CallType CallType => CallType.Infix;

        public string Symbol => "or";

        public object EvaluateList(FsList pars)
        {
            for (int i = 0; i < pars.Length; i++)
            {
                var par = pars[i];

                if (!(par is bool b))
                    return new FsError(FsError.ERROR_TYPE_MISMATCH,
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