using System.Collections;
using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class FindFirstFunction : IFsFunction
    {
        private const int MaxParameterCount = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "First";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameterCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameterCount}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction func))
                throw new error.TypeMismatchError($"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var lst = (FsList)par0;

            for (int i = 0; i < lst.Length; i++)
            {
                var result = func.EvaluateList(new ParameterList { X = lst[i], I = i });

                if (result is bool && (bool)result)
                    return lst[i];
            }

            return null;
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "List";
                case 1:
                    return "Filter Function";
                default:
                    return "";
            }
        }
        
        class ParameterList : FsList
        {
            public object X;
            public object I;

            public int Length => 2;
            public IEnumerator<object> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public object this[int index] => index switch
            {
                0 => X,
                1 => I,
                _ => null,
            };
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            
        }
    }
}