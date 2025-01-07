using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class SplitFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "split";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol} requires at least one parameter.");

            var input = pars[0] as string;
            if (input == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol}: first parameter must be a string");

            var separator = (pars.Length > 1 ? pars[1] as string : null) ?? "";
            var parts = input.Split(new[] { separator }, StringSplitOptions.None);

            var result = new List<string>();
            foreach (var p in parts)
                result.Add(p);
            return new ArrayFsList(result);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "string",
                1 => "separator",
                _ => ""
            };
        }
    }
}
