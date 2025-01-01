using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetKvcExpression(ParseContext context, int index)
        {
            var syntaxErrors = context.SyntaxErrors;
            var i = SkipSpace(context, index).NextIndex;
            var nodeStart = i;
            int i2 = GetLiteralMatch(context, i, "{").NextIndex;
            if (i2 == i)
                return new ExpressionBlockResult(null, null, index);
            i = i2;
            (var block, var parseNode, i2) = GetKvcBody(context, i,true);
            i = SkipSpace(context, i2).NextIndex;
            i2 = GetLiteralMatch(context, i, "}").NextIndex;
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "'}' expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = i2;
            parseNode.Pos = nodeStart;
            parseNode.Length = i - nodeStart;
            return new ExpressionBlockResult(block, parseNode, i);
        }

        static ExpressionBlockResult GetNakedKvc(ParseContext context, int index)
        {
            return GetKvcBody(context, index, false);
        }

        static ExpressionBlockResult GetKvcBody(ParseContext context, int index, bool allowKeyOnly)
        {
            var syntaxErrors = context.SyntaxErrors;
            ParseNode parseNode = null;
            KvcExpression kvcExpr = null;
            var i = SkipSpace(context, index).NextIndex;
            var kvs = new List<KvcExpression.KeyValueExpression>();
            var nodeItems = new List<ParseNode>();
            ExpressionBlock retExp = null;
            do
            {
                int i2;
                if (kvs.Count > 0 || retExp != null)
                {
                    i2 = GetLiteralMatchMultiple(context, i, new[] { ",", ";" }).NextIndex;
                    if (i2 == i)
                        break;
                    i = SkipSpace(context, i2).NextIndex;
                }

                var kvcItemResult = GetKvcItem(context, i, allowKeyOnly);
                i2 = kvcItemResult.NextIndex;
                var otherItem = kvcItemResult.Item;
                var nodeOtherItem = kvcItemResult.ParseNode;

                if (i2 == i)
                    break;
                if (otherItem.Key == null)
                {
                    if (retExp != null)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(nodeOtherItem.Pos, nodeOtherItem.Length, "Duplicate return statement"));
                        return new ExpressionBlockResult(null, null, index);
                    }

                    retExp = otherItem.ValueExpression;
                }
                else
                    kvs.Add(otherItem);

                nodeItems.Add(nodeOtherItem);
                i = SkipSpace(context, i2).NextIndex;
            } while (true);

            kvcExpr = new KvcExpression();
            // Removed the kvcExpr.SetContext(context.ReferenceProvider) call
            var error = kvcExpr.SetKeyValues(kvs.ToArray(), retExp);
            if (error != null)
            {
                syntaxErrors.Add(new SyntaxErrorData(index, i - index, error));
                return new ExpressionBlockResult(null, null, index);
            }

            parseNode = new ParseNode(ParseNodeType.KeyValueCollection, index, i - index, nodeItems);
            return new ExpressionBlockResult(kvcExpr, parseNode, i);
        }
    }
}