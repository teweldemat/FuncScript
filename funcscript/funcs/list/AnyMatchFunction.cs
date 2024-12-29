using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class AnyMatchFunction : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Any";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null)
                return false;

            if (!(par0 is FsList))
                throw new error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction func))
                throw new error.TypeMismatchError($"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var lst = (FsList)par0;

            for (int i = 0; i < lst.Length; i++)
            {
                var result = func.EvaluateList(new ArrayFsList( new object[] { lst[i], i }));

                if (result is bool && (bool)result)
                    return true;
            }

            return false;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Filter Function",
                _ => ""
            };
        }
    }
}