using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class SplitFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "split";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.EvaluationTimeException($"{Symbol} requires at least one parameter.");

            var input = pars[0] as string;
            if (input == null)
                return null;

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