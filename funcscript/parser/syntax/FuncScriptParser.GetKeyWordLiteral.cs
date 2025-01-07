using FuncScript.Model;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record GetKeyWordLiteralResult(object Literal, ParseNode ParseNode, int NextIndex)
            : ParseResult(ParseNode, NextIndex);

        static GetKeyWordLiteralResult GetKeyWordLiteral(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            int i = GetLiteralMatch(context, index, "null").NextIndex;
            object literal;

            if (i > index)
            {
                literal = null;
            }
            else if ((i = GetLiteralMatch(context, index, "true").NextIndex) > index)
            {
                literal = true;
            }
            else if ((i = GetLiteralMatch(context, index, "false").NextIndex) > index)
            {
                literal = false;
            }
            else
            {
                return new GetKeyWordLiteralResult(null, null, index);
            }

            parseNode = new ParseNode(ParseNodeType.KeyWord, index, i - index);
            return new GetKeyWordLiteralResult(literal, parseNode, i);
        }
    }
}
