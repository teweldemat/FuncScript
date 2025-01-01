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
            if (res1.NextIndex == context.Expression.Trim().Length)
                return res1;

            context.SyntaxErrors.Clear();
            return GetExpression(context, 0);
        }
    }
}