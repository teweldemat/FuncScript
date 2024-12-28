using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetSpaceLessStringResult(string Text, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode,NextIndex);

        static GetSpaceLessStringResult GetSpaceLessString(ParseContext context, int index)
        {
            if (index >= context.Expression.Length)
                return new GetSpaceLessStringResult(null, null, index);

            var i = index;

            if (i >= context.Expression.Length || isCharWhiteSpace(context.Expression[i]))
                return new GetSpaceLessStringResult(null, null, index);

            i++;
            while (i < context.Expression.Length && !isCharWhiteSpace(context.Expression[i]))
                i++;

            var text = context.Expression.Substring(index, i - index);
            var parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
            return new GetSpaceLessStringResult(text, parseNode, i);
        }
    }
}