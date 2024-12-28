using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetInfixExpression(ParseContext context, int index)
        {
            var result = GetInfixExpressionSingleLevel(context, s_operatorSymols.Length - 1, s_operatorSymols[^1], index);
            
            if (result.Expression != null)
            {
                result.Expression.SetContext(context.Provider);
            }

            return result;
        }
    }
}
