using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetKeyValuePair(KeyValueCollection provider, string exp, int index,
            out KvcExpression.KeyValueExpression keyValue, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            keyValue = null;
            string name;
            string nameLower;
            var i = GetSimpleString(provider, exp, index, out name, out var nodeNeme, new List<SyntaxErrorData>());
            if (i == index)
            {
                i = GetIdentifier(provider, exp, index, false, out name, out nameLower, out _, out nodeNeme);
                if (i == index)
                    return index;
            }
            else
                nameLower = name.ToLower();
            
            i = SkipSpace(exp, i);

            var i2 = GetLiteralMatch(exp, i, ":");
            if (i2 == i)
            {
                keyValue = new KvcExpression.KeyValueExpression
                {
                    Key = name,
                    KeyLower = nameLower,
                    ValueExpression = new ReferenceBlock(name, nameLower, true)
                    
                };
                nodeNeme.NodeType = ParseNodeType.Key;
                parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, new[] { nodeNeme });
                return i;
            }

            i = i2;

            i = SkipSpace(exp, i);
            i2 = GetExpression(provider, exp, i, out var expBlock, out var nodeExpBlock, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "value expression expected"));
                return index;
            }

            i = i2;
            i = SkipSpace(exp, i);
            keyValue = new KvcExpression.KeyValueExpression
            {
                Key = name,
                ValueExpression = expBlock
            };
            nodeNeme.NodeType = ParseNodeType.Key;
            parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, new[] { nodeNeme, nodeExpBlock });
            return i;
        }
    }
}