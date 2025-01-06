using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
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
        

            context.SyntaxErrors.Clear();
            return GetExpression(context, 0);
        }
    }
}