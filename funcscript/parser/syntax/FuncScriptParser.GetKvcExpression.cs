using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static ExpressionBlockResult GetKvcExpression(ParseContext context, bool nakdeMode, int index)
        {
            var syntaxErrors = context.SyntaxErrors;
            ParseNode parseNode = null;
            KvcExpression kvcExpr = null;
            var i = SkipSpace(context, index).NextIndex;
            int i2;
            if (!nakdeMode)
            {
                i2 = GetLiteralMatch(context, i, "{").NextIndex;
                if (i2 == i)
                    return new ExpressionBlockResult(null, null, index);
                i = SkipSpace(context, i2).NextIndex;
            }

            var kvs = new List<KvcExpression.KeyValueExpression>();
            var nodeItems = new List<ParseNode>();
            ExpressionBlock retExp = null;
            do
            {
                if (kvs.Count > 0 || retExp != null)
                {
                    i2 = GetLiteralMatch(context, i, ",", ";").NextIndex;
                    if (i2 == i)
                        break;
                    i = SkipSpace(context, i2).NextIndex;
                }

                var kvcItemResult = GetKvcItem(context, nakdeMode,  i);
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

            if (!nakdeMode)
            {
                i2 = GetLiteralMatch(context, i, "}").NextIndex;
                if (i2 == i)
                {
                    syntaxErrors.Add(new SyntaxErrorData(i, 0, "'}' expected"));
                    return new ExpressionBlockResult(null, null, index);
                }

                i = SkipSpace(context, i2).NextIndex;
            }

            if (nakdeMode)
            {
                if (kvs.Count == 0 && retExp == null)
                    return new ExpressionBlockResult(null, null, index);
            }

            kvcExpr = new KvcExpression();
            kvcExpr.SetContext(context.ReferenceProvider);
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