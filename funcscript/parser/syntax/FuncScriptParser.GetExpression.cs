using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetExpression(ParseContext context, int index)
        {
            var result = GetInfixExpression(context, index);
            if (result.NextIndex > index)
                return result;

            return new ParseResult(null, null, index);
        }
    }
}