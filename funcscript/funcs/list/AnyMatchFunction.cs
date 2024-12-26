using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class AnyMatchFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Any";

        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected {this.MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);

            if (par0 == null)
                return false;

            if (!(par0 is FsList))
                throw new error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction func))
                throw new error.TypeMismatchError($"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var lst = (FsList)par0;

            for (int i = 0; i < lst.Length; i++)
            {
                var result = func.Evaluate(parent, new ParameterList { X = lst[i], I = i });

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

        private class ParameterList : IParameterList
        {
            public object X;
            public object I;

            public override int Count => 2;

            public override (object, CodeLocation) GetParameterWithLocation(IFsDataProvider provider, int index)
            {
                return index switch
                {
                    0 => (X, null),
                    1 => (I, null),
                    _ => (null, null)
                };
            }
        }
    }
}