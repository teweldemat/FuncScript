using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{

    public class AndFunction : IFsFunction
    {
        private const int MaxParameters = -1;

        public CallType CallType => CallType.Infix;

        public string Symbol => "and";

        public object EvaluateList(FsList pars)
        {
            int count = pars.Length;

            for (int i = 0; i < count; i++)
            {
                var par = pars[i];

                if (!(par is bool b))
                    return new FsError(FsError.ERROR_TYPE_MISMATCH,
                        $"{this.Symbol} doesn't apply to this type:{(par == null ? "null" : par.GetType())} ");

                if(!b)
                    return false;
            }

            return true;
        }


        public string ParName(int index)
        {
            return $"Value {index + 1}";
        }
    }
}