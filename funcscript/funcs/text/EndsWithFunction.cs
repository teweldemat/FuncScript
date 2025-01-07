using FuncScript.Core;
using FuncScript.Model;
using System;

namespace FuncScript.Funcs.Strings
{
    internal class EndsWithFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "endswith";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                throw new Error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null || par1 == null)
                return false;

            if (!(par0 is string) || !(par1 is string))
                throw new Error.TypeMismatchError($"Function {this.Symbol}. Both parameters must be strings");

            var mainString = (string)par0;
            var ending = (string)par1;

            return mainString.EndsWith(ending, StringComparison.Ordinal);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "main string",
                1 => "ending substring",
                _ => null
            };
        }
    }
}
