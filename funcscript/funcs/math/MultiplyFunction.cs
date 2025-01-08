using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class MultiplyFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "*";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: at least one parameter expected");

            bool isNull = true, isInt = false, isLong = false, isDouble = false;
            int intTotal = 1;
            long longTotal = 1;
            double doubleTotal = 1;
            int count = pars.Length;

            for (int i = 0; i < count; i++)
            {
                var d = pars[i];

                if (isNull)
                {
                    if (d is int)
                        isInt = true;
                    else if (d is long)
                        isLong = true;
                    else if (d is double)
                        isDouble = true;
                    else
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                    
                    isNull = false;
                }

                if (isInt)
                {
                    if (d is int)
                    {
                        intTotal *= (int)d;
                    }
                    else if (d is long)
                    {
                        isInt = false;
                        isLong = true;
                        longTotal = intTotal * (long)d;
                    }
                    else if (d is double)
                    {
                        isInt = false;
                        isDouble = true;
                        doubleTotal = intTotal * (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                    }
                    continue;
                }

                if (isLong)
                {
                    if (d is int)
                    {
                        longTotal *= (long)(int)d;
                    }
                    else if (d is long)
                    {
                        longTotal *= (long)d;
                    }
                    else if (d is double)
                    {
                        isLong = false;
                        isDouble = true;
                        doubleTotal = longTotal * (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                    }
                    continue;
                }

                if (isDouble)
                {
                    if (d is int)
                    {
                        doubleTotal *= (double)(int)d;
                    }
                    else if (d is long)
                    {
                        doubleTotal *= (double)(long)d;
                    }
                    else if (d is double)
                    {
                        doubleTotal *= (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                    }
                    continue;
                }
            }

            if (isDouble)
                return doubleTotal;

            if (isLong)
                return longTotal;

            if (isInt)
                return intTotal;

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}
