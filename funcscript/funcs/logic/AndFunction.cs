using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{

    public class AndFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "and";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            int count = pars.Length;

            for (int i = 0; i < count; i++)
            {
                var par = pars[i];

                if (!(par is bool b))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
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
