using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        record GetKeyValuePairResult(KvcExpression.KeyValueExpression KeyValue, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode,NextIndex);

        static GetKeyValuePairResult GetKeyValuePair(ParseContext context, int index,bool allowKeyOnly)
        {
            ParseNode parseNode = null;
            KvcExpression.KeyValueExpression keyValue = null;
            string name;
            string nameLower;
            ( name,var nodeNeme,var i) = GetSimpleString(context, index);
            if (i == index)
            {
                (name,nameLower,_,nodeNeme,i) = GetIdentifier(context, index, false);
                if (i == index)
                    return new GetKeyValuePairResult(null, null, index);
            }
            else
                nameLower = name.ToLower();
            
            i = SkipSpace(context, i).NextIndex;

            var i2 = GetLiteralMatch(context, i, ":").NextIndex;
            if (i2 == i)
            {
                if (!allowKeyOnly)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "':' expected"));
                    return new GetKeyValuePairResult(null, null, index);
                }

                keyValue = new KvcExpression.KeyValueExpression
                {
                    Key = name,
                    KeyLower = nameLower,
                    ValueExpression = new ReferenceBlock(name, nameLower, true)
                };
                nodeNeme.NodeType = ParseNodeType.Key;
                parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, new[] { nodeNeme });
                return new GetKeyValuePairResult(keyValue, parseNode, i);
            }

            i = i2;

            i = SkipSpace(context, i).NextIndex;
            (var expBlock,var nodeExpBlock,i2) = GetExpression(context, i);
            if (i2 == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "value expression expected"));
                return new GetKeyValuePairResult(null, null, index);
            }

            i = i2;
            i = SkipSpace(context, i).NextIndex;
            keyValue = new KvcExpression.KeyValueExpression
            {
                Key = name,
                ValueExpression = expBlock
            };
            nodeNeme.NodeType = ParseNodeType.Key;
            parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, new[] { nodeNeme, nodeExpBlock });
            return new GetKeyValuePairResult(keyValue, parseNode, i);
        }
    }
}