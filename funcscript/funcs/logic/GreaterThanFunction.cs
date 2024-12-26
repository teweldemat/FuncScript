using funcscript.core;
using funcscript.model;
using System;

namespace funcscript.funcs.logic
{
    public class GreaterThanFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => ">";

        public int Precidence => 200;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MaxParsCount)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {this.MaxParsCount} got {pars.Count}");

            var parBuilder = new CallRefBuilder(this,parent, pars);
            var par0 = parBuilder.GetParameter(0);
            var par1 = parBuilder.GetParameter(1);

            if (par0 is ValueReferenceDelegate || par1 is ValueReferenceDelegate)
                return parBuilder.CreateRef();

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null || par1 == null)
                return null;

            if (FuncScript.IsNumeric(par0) && FuncScript.IsNumeric(par1))
            {
                FuncScript.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if (par0.GetType() != par1.GetType())
                return new FsError(FsError.ERROR_TYPE_MISMATCH, $"{this.Symbol}: Can't compare incompatible types");

            if (par0 is IComparable comparable)
                return comparable.CompareTo(par1) > 0;
            return new FsError(FsError.ERROR_TYPE_MISMATCH, $"{this.Symbol} function can't compare these data types: {par0.GetType()}");

        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Left Value";
                case 1:
                    return "Right Value";
                default:
                    return "";
            }
        }
    }
}