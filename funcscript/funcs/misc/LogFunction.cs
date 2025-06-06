using FuncScript.Core;
using FuncScript.Host;
using FuncScript.Model;

namespace FuncScript.Funcs.Misc
{
    public class LogFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;
        public string Symbol => "log";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: {this.ParName(0)} expected");

            var anchor = pars[0];
            var second = pars.Length > 1 ? pars[1] : null;

            if (second is IFsFunction func)
            {
                var result = func.EvaluateList(context,new ArrayFsList(new object[] { anchor }));
                FsLogger.DefaultLogger.WriteLine(result?.ToString() ?? "<null>");
            }
            else
            {
                FsLogger.DefaultLogger.WriteLine(second?.ToString() ?? "<null>");
            }
            return anchor;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "anchor",
                1 => "valueOrFunction",
                _ => null
            };
        }
    }
}
