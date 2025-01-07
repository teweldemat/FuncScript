using FuncScript.Model;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record GetIntResult(string IntVal, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode, NextIndex);

        static GetIntResult GetInt(ParseContext context, bool allowNegative, int index)
        {
            ParseNode parseNode = null;
            int i = index;
            if (allowNegative)
                i = GetLiteralMatch(context, i, "-").NextIndex;

            var i2 = i;
            while (i2 < context.Expression.Length && char.IsDigit(context.Expression[i2]))
                i2++;

            if (i == i2)
            {
                return new GetIntResult(null, parseNode, index);
            }

            i = i2;

            string intVal = context.Expression.Substring(index, i - index);
            parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
            return new GetIntResult(intVal, parseNode, i);
        }
    }
}
