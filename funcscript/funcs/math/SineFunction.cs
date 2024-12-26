using funcscript.core;

namespace funcscript.funcs.math
{
    public class SineFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Sin";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            var val = pars.GetParameter(parent, 0);

            if (val is int intValue)
            {
                return Math.Sin((double)intValue);
            }

            if (val is double doubleValue)
            {
                return Math.Sin(doubleValue);
            }

            if (val is long longValue)
            {
                return Math.Sin((double)longValue);
            }

            throw new error.TypeMismatchError($"{this.Symbol}: A number was expected.");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }

    public class CosineFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Cos";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            var val = pars.GetParameter(parent, 0);
            if (val is int intValue)
            {
                return Math.Cos((double)intValue);
            }
            if (val is double doubleValue)
            {
                return Math.Cos(doubleValue);
            }
            if (val is long longValue)
            {
                return Math.Cos((double)longValue);
            }
            throw new error.TypeMismatchError($"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}