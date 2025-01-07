using System.Collections;
using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.List
{
    public class FindFirstFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "First";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction func))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

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
