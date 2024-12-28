using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetRootExpression(ParseContext context, int index)
        {
            var thisErrors = new List<SyntaxErrorData>();
            var result = GetExpression(context, index);
            if (result.NextIndex > index)
            {
                result.Expression.SetContext(context.Provider);
                context.Serrors.AddRange(thisErrors);
                return result;
            }
            return new ParseResult(null, null, index);
            /*
            var thisErrors = new List<SyntaxErrorData>();
            var kvcResult = GetKvcExpression(new ParseContext(context.Provider, context.Expression, thisErrors), false, index);
            if (kvcResult.NextIndex > index)
            {
                kvcResult.Expression.SetContext(context.Provider);
                context.Serrors.AddRange(thisErrors);
                return kvcResult;
            }

            thisErrors = new List<SyntaxErrorData>();
            var expressionResult = GetExpression(context, index);
            if (expressionResult.NextIndex > index)
            {
                expressionResult.Expression.SetContext(context.Provider);
                context.Serrors.AddRange(thisErrors);
                return expressionResult;
            }
            return new ParseResult(null, null, index);
            */
        }
    }
}