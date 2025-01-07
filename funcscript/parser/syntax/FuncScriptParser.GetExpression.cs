using FuncScript.Model;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetExpression(ParseContext context, int index)
        {
            var result = GetInfixExpression(context, index);
            if (result.NextIndex > index)
                return result;

            return new ExpressionBlockResult(null, null, index);
        }
    }
}
