using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.List
{
    public class SkipFunction : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Skip";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new Error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new Error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is int))
                throw new Error.TypeMismatchError($"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var lst = (FsList)par0;
            int n = (int)par1;

            if (n <= 0)
                return lst;

            if (n >= lst.Length)
                return new ArrayFsList(new object[] { });

            return new ArrayFsList(lst.Skip(n).ToArray());
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "List";
                case 1:
                    return "Number";
                default:
                    return "";
            }
        }
    }
}
