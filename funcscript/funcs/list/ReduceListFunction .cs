using funcscript.core;
using funcscript.model;
using System;

namespace funcscript.funcs.list
{
    public class ReduceListFunction : IFsFunction
    {
        public int MaxParsCount => 3;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Reduce";

        class DoListFuncPar : IParameterList
        {
            public object S;
            public object X;
            public object I;

            public override int Count => 3;

            public override (object, CodeLocation) GetParameterWithLocation(IFsDataProvider provider, int index)
            {
                return index switch
                {
                    0 => (X, null),
                    1 => (S, null),
                    2 => (I, null),
                    _ => (null, null),
                };
            }
        }

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            var par0 = pars.GetParameter(parent, 0);
            if (par0 is null) return null;

            var par1 = pars.GetParameter(parent, 1);

            var par2 = pars.GetParameter(parent, 2);

            return EvaluateInternal(parent, par0, par1, par2, false);
        }

        private object EvaluateInternal(IFsDataProvider parent, object par0, object par1, object par2, bool dref)
        {
            if (!(par0 is FsList lst))
                throw new error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            var func = par1 as IFsFunction;
            if (func == null)
                throw new error.TypeMismatchError($"{this.Symbol} function: The second parameter didn't evaluate to a function");

            var total = par2;
            
            for (int i = 0; i < lst.Length; i++)
            {
                total = func.Evaluate(parent, new DoListFuncPar { S = total, X = lst[i], I = i });
            }

            return FuncScript.NormalizeDataType(total);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Transform Function",
                _ => ""
            };
        }
    }
}