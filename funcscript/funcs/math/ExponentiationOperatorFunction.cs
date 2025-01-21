using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class ExponentiationOperator : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "^";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            // Check if there are sufficient parameters
            if (pars.Length < 2)
            {
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: at least two parameters expected");
            }

            // Start with the base value
            var baseValue = pars[0];
            double result;

            // Determine the type of the base
            if (baseValue is int baseInt)
            {
                result = baseInt;
            }
            else if (baseValue is long baseLong)
            {
                result = baseLong;
            }
            else if (baseValue is double baseDouble)
            {
                result = baseDouble;
            }
            else
            {
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as the first parameter");
            }

            // Iterate through the rest of the parameters
            for (int i = 1; i < pars.Length; i++)
            {
                var exponentValue = pars[i];
                
                if (exponentValue is int exponentInt)
                {
                    result = System.Math.Pow(result, exponentInt);
                }
                else if (exponentValue is long exponentLong)
                {
                    result = System.Math.Pow(result, exponentLong);
                }
                else if (exponentValue is double exponentDouble)
                {
                    result = System.Math.Pow(result, exponentDouble);
                }
                else
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as parameter {i + 1}");
                }
            }

            return result;
        }

       
    }
}
