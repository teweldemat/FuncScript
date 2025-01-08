using FuncScript.Block;
using FuncScript.Error;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetRootExpression(ParseContext context)
        {
            var res1 = GetNakedKvc(context, 0);
            if (res1.NextIndex > 0)
            {
                var next = SkipSpace(context, res1.NextIndex);
                if (next.NextIndex == context.Expression.Length)
                    return res1;
            }

            {
                context.SyntaxErrors.Clear();
                var next = GetExpression(context, 0);
                if (next.NextIndex > 0)
                {
                    var end = SkipSpace(context, next.NextIndex);
                    if (end.NextIndex < context.Expression.Length)
                    {
                        context.SyntaxErrors.Clear();
                        context.SyntaxErrors.Add(new SyntaxErrorData(next.NextIndex,
                            context.Expression.Length - next.NextIndex,"Unrecognized code"));
                        return new ExpressionBlockResult(null, null, 0);
                    }
                }
                return next;
            }
        }
    }
}