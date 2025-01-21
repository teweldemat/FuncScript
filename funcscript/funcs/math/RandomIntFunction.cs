using FuncScript.Core;
using FuncScript.Model;
using System;

namespace FuncScript.Funcs.Math
{
    public class RandomIntFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "RandomInt";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
            {
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 2 parameters got {pars.Length}");
            }

            // Parse the minimum and maximum values
            var minVal = pars[0];
            var maxVal = pars[1];

            if (minVal is int minInt && maxVal is int maxInt)
            {
                if (minInt > maxInt)
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"{this.Symbol}: min value should be less than or equal to max value.");
                }
                Random random = new Random();
                return random.Next(minInt, maxInt);
            }

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: both parameters should be integers.");
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "min",
                1 => "max",
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Invalid parameter index")
            };
        }
    }
}
