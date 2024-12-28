using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult Parse(ParseContext context)
        {
            var rootExpressionResult = GetRootExpression(context, 0);
            if (rootExpressionResult.Block != null)
            {
                rootExpressionResult.Block.SetContext(context.ReferenceProvider);
            }
            return rootExpressionResult;
        }
    }
}