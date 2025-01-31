using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.math
{
    public class DivisionFunction : IFsFunction
    {
        private const int MaxParameters = -1; // Replacing MaxParsCount property with a constant

        public CallType CallType => CallType.Infix;

        public string Symbol => "/";

        public object EvaluateList(FsList pars)
        {
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
                    isInt = true;
                    intTotal = 1;
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