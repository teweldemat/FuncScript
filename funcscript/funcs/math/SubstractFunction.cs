using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class SubstractFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "-";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: at least one parameter expected");

            var ret = EvaluateInternal(pars, (i) => 
            {
                if (i >= pars.Length) return (false, null);
                var ret = pars[i];
                return (true, ret);
            });
            if (ret == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
            
            return ret;
        }

        object EvaluateInternal(FsList pars, Func<int, (bool, object)> getPar)
        {
            bool isInt = false, isLong = false, isDouble = false;
            int intTotal = 0;
            long longTotal = 0;
            double doubleTotal = 0;
            int count = pars.Length;

            if (count > 0)
            {
                var p = getPar(0);
                if (!p.Item1)
                    return null;
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
                    return null;
                }
            }

            for (int i = 1; i < count; i++)
            {
                var p = getPar(i);
                if (!p.Item1)
                    return null;
                var d = p.Item2;

                if (isInt)
                {
                    if (d is int)
                    {
                        intTotal -= (int)d;
                    }
                    else if (d is long)
                    {
                        isInt = false;
                        isLong = true;
                        longTotal = intTotal;
                        longTotal -= (long)d;
                    }
                    else if (d is double)
                    {
                        isInt = false;
                        isDouble = true;
                        doubleTotal = intTotal;
                        doubleTotal -= (double)d;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (isLong)
                {
                    if (d is int)
                    {
                        longTotal -= (long)(int)d;
                    }
                    else if (d is long)
                    {
                        longTotal -= (long)d;
                    }
                    else if (d is double)
                    {
                        isLong = false;
                        isDouble = true;
                        doubleTotal = longTotal;
                        doubleTotal -= (double)d;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (isDouble)
                {
                    if (d is int)
                    {
                        doubleTotal -= (double)(int)d;
                    }
                    else if (d is long)
                    {
                        doubleTotal -= (double)(long)d;
                    }
                    else if (d is double)
                    {
                        doubleTotal -= (double)d;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (isDouble)
                return doubleTotal;
            if (isLong)
                return longTotal;
            if (isInt)
                return intTotal;

            return null;
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}
