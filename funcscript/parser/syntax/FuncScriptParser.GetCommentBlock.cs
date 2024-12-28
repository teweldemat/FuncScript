namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetCommentBlockResult(ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode,NextIndex);
        static GetCommentBlockResult GetCommentBlock(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            var i = GetLiteralMatch(context, index, new string[] { "//" }).NextIndex;
            if (i == index)
                return new GetCommentBlockResult(parseNode, index);
            var i2 = context.Expression.IndexOf("\n", i,StringComparison.InvariantCulture);
            if (i2 == -1)
                i = context.Expression.Length;
            else
                i = i2 + 1;
            parseNode = new ParseNode(ParseNodeType.Comment, index, i - index);
            return new GetCommentBlockResult(parseNode, i);
        }
    }
}