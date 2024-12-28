using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ParseResult Parse(ParseContext context)
        {
            var rootExpressionResult = GetRootExpression(context, 0);
            if (rootExpressionResult.Expression != null)
            {
                rootExpressionResult.Expression.SetContext(context.Provider);
            }
            return rootExpressionResult;
        }
    }
}