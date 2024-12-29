using System;
using System.Text.RegularExpressions;
using funcscript.core;
using funcscript.model;

namespace funcscript.openai
{
    public class FindCodeBlocksFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "findCodeBlocks";

        record LangCode(string lang, string code);
        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.EvaluationTimeException($"{Symbol} requires at least one parameter.");

            var input = pars[0] as string;
            if (input == null)
                return null;

            var codeBlockRegex = new Regex(@"```(\S*)\s*([\s\S]*?)```", RegexOptions.Multiline);
            var matches = codeBlockRegex.Matches(input);
            var result = new List<LangCode>();

            
            foreach (Match match in matches)
            {
                var language = match.Groups[1].Value;
                var code = match.Groups[2].Value;
                var pair = new LangCode(language, code);
                result.Add(pair);
            }

            return new ArrayFsList(result);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "string",
                _ => ""
            };
        }
    }
}