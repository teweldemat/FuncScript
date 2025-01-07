using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetInfixExpression(ParseContext context, int index)
        {
            var result = GetInfixExpressionSingleLevel(context, s_operatorSymbols.Length - 1, s_operatorSymbols[^1], index);

            // Removed the call to SetContext as it is no longer needed
            return result;
        }
    }
}
