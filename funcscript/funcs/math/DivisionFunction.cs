using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class DivisionFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "/";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: at least one parameter expected");

            var ret = EvaluateInternal(pars, i => (true, pars[i]));

            return ret;
        }

        object EvaluateInternal(FsList pars, Func<int, (bool, object)> getPar)
        {
            bool isInt = false, isLong = false, isDouble = false;
            int intTotal = 1;
            long longTotal = 1;
            double doubleTotal = 1;
            int count = pars.Length;

            if (count > 0)
            {
                var p = getPar(0);
                if (!p.Item1)
                    return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: invalid parameter");
                var d = p.Item2;

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
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                }
            }

            for (int i = 1; i < count; i++)
            {
                var p = getPar(i);
                if (!p.Item1)
                    return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: invalid parameter");
                var d = p.Item2;

                if (d.Equals(0))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: division by zero");

                if (isInt)
                {
                    if (d is int)
                    {
                        intTotal /= (int)d;
                    }
                    else if (d is long)
                    {
                        isLong = true;
                        longTotal = intTotal;
                    }
                    else if (d is double)
                    {
                        isDouble = true;
                        doubleTotal = intTotal;
                    }
                    else
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                }

                if (isLong)
                {
                    if (d is int)
                    {
                        longTotal /= (long)(int)d;
                    }
                    else if (d is long)
                    {
                        longTotal /= (long)d;
                    }
                    else if (d is double)
                    {
                        isDouble = true;
                        doubleTotal = longTotal;
                    }
                    else
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");

                }

                if (isDouble)
                {
                    if (d is int)
                    {
                        doubleTotal /= (double)(int)d;
                    }
                    else if (d is long)
                    {
                        doubleTotal /= (double)(long)d;
                    }
                    else if (d is double)
                    {
                        doubleTotal /= (double)d;
                    }
                    else
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");

                }
            }

            if (isDouble)
                return doubleTotal;
            if (isLong)
                return longTotal;
            if (isInt)
                return intTotal;

            return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: evaluation error");
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}
