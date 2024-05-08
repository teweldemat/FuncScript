using funcscript.core;
using funcscript.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace funcscript.funcs.list
{
    public class LengthFunction : IFsFunction
    {
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Len";

        public int Precidence => 0;
        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected {this.MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            if (par0 is ValueReferenceDelegate)
                return CallRef.Create(parent, this, pars);

            return par0 switch
            {
                null => 0,
                FsList list => list.Data.Length,
                string s => s.Length,
                _ => throw new error.TypeMismatchError($"{this.Symbol} function doesn't apply to {par0.GetType()}")
            };
        }

        public string ParName(int index)
        {
            switch(index)
            {
                case 0:
                    return "List";
                default:
                    return "";
            }
        }
    }
}
