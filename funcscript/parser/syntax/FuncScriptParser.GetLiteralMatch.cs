using ArgumentNullException = System.ArgumentNullException;
using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        public record GetLiteralMatchResult(string Matched, int NextIndex);
        static GetLiteralMatchResult GetLiteralMatch(ParseContext context, int index, params string[] provider)
        {
            return GetLiteralMatch(context.Expression, index, provider);
        }
        public static GetLiteralMatchResult GetLiteralMatch(String expression, int index, params string[] provider)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "The input expression cannot be null.");
            }

            string matched = null;
            foreach (var k in provider)
            {
                bool matchFound = true;
                if (index + k.Length <= expression.Length)
                {
                    for (int i = 0; i < k.Length; i++)
                    {
                        if (char.ToLowerInvariant(expression[index + i]) != char.ToLowerInvariant(k[i]))
                        {
                            matchFound = false;
                            break;
                        }
                    }

                    if (matchFound)
                    {
                        matched = k.ToLowerInvariant();
                        return new GetLiteralMatchResult(matched, index + k.Length);
                    }
                }
            }

            return new GetLiteralMatchResult(matched, index);
        }
    }
}
