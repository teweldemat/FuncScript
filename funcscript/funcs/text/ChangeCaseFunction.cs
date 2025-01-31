using funcscript.core;
using funcscript.model;
using System.Linq;
using System.Globalization;
using System.Text;

namespace funcscript.funcs.text
{
    public class ChangeCaseFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "changecase";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 2)
                throw new error.EvaluationTimeException($"{this.Symbol} requires at least two parameters: text and case type.");

            var input = pars[0] as string;
            var caseType = pars[1] as string;

            if (input == null || caseType == null)
                throw new error.EvaluationTimeException($"{this.Symbol} requires the first parameter to be a string and the second parameter to specify a valid case type.");

            return caseType.ToLower() switch
            {
                "lower"     => input.ToLower(),
                "upper" => input.ToUpper(),
                "pascal" => ToPascalCase(input),
                "snake"  => ToSnakeCase(input),
                "kebab"  => ToKebabCase(input),
                _ => throw new error.EvaluationTimeException(
                        $"{this.Symbol} does not support the case type '{caseType}'. " +
                        "Supported types: lower, upper, pascal, snake, kebab."
                     )
            };
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "text",
                1 => "case",
                _ => ""
            };
        }

        private string ToPascalCase(string input)
        {
            var words = input.Split(new[] { ' ', '_', '-', '.', ',' },
                                    System.StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            foreach (var word in words)
            {
                if (word.Length == 0) continue;
                sb.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                    sb.Append(word.Substring(1));
            }
            return sb.ToString();
        }

        private string ToSnakeCase(string input)
        {
            var words = input.Split(new[] { ' ', '_', '-', '.', ',' },
                                    System.StringSplitOptions.RemoveEmptyEntries);
            return string.Join("_", words.Select(w => w.ToLower()));
        }

        private string ToKebabCase(string input)
        {
            var words = input.Split(new[] { ' ', '_', '-', '.', ',' },
                                    System.StringSplitOptions.RemoveEmptyEntries);
            return string.Join("-", words.Select(w => w.ToLower()));
        }
    }
}