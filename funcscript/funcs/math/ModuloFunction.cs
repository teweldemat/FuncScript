using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class ModuloFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "%";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            bool isInt = false, isLong = false, isDouble = false;
            int intTotal = 1;
            long longTotal = 1;
            double doubleTotal = 1;
            int count = pars.Length;

            if (count == 0)
            {
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: at least one parameter expected");
            }

            var d = pars[0];

            if (d is int)
            {
                isInt = true;
                intTotal = (int)d;
            }
            else if (d is long)
            {
                isLong = true;
                longTotal = (long)d;
            }
            else if (d is double)
            {
                isDouble = true;
                doubleTotal = (double)d;
            }
            else
            {
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as the first parameter");
            }

            for (int i = 1; i < count; i++)
            {
                d = pars[i];

                // Check for division by zero before performing modulo
                if ((d is int intDiv && intDiv == 0) ||
                    (d is long longDiv && longDiv == 0L) ||
                    (d is double doubleDiv && doubleDiv == 0.0))
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: division by zero at parameter {i + 1}");
                }

                if (isInt)
                {
                    if (d is int)
                    {
                        intTotal %= (int)d;
                    }
                    else if (d is long)
                    {
                        isInt = false;
                        isLong = true;
                        longTotal = intTotal;
                        longTotal %= (long)d;
                    }
                    else if (d is double)
                    {
                        isInt = false;
                        isDouble = true;
                        doubleTotal = intTotal;
                        doubleTotal %= (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as parameter {i + 1}");
                    }
                }
                else if (isLong)
                {
                    if (d is int)
                    {
                        longTotal %= (long)(int)d;
                    }
                    else if (d is long)
                    {
                        longTotal %= (long)d;
                    }
                    else if (d is double)
                    {
                        isLong = false;
                        isDouble = true;
                        doubleTotal = longTotal;
                        doubleTotal %= (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as parameter {i + 1}");
                    }
                }
                else if (isDouble)
                {
                    if (d is int)
                    {
                        doubleTotal %= (double)(int)d;
                    }
                    else if (d is long)
                    {
                        doubleTotal %= (double)(long)d;
                    }
                    else if (d is double)
                    {
                        doubleTotal %= (double)d;
                    }
                    else
                    {
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected as parameter {i + 1}");
                    }
                }
            }

            if (isDouble)
                return doubleTotal;

            if (isLong)
                return longTotal;

            if (isInt)
                return intTotal;

            return new FsError(FsError.ERROR_DEFAULT, $"{this.Symbol}: unexpected error");
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}