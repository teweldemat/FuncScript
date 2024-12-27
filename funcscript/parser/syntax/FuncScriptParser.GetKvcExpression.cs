using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetKvcExpression(KeyValueCollection provider, bool nakdeMode, String exp, int index,
            out KvcExpression kvcExpr,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            kvcExpr = null;
            var i = SkipSpace(exp, index);
            int i2;
            if (!nakdeMode)
            {
                i2 = GetLiteralMatch(exp, i, "{");
                if (i2 == i)
                    return index;
                i = SkipSpace(exp, i2);
            }

            var kvs = new List<KvcExpression.KeyValueExpression>();
            var nodeItems = new List<ParseNode>();
            ExpressionBlock retExp = null;
            do
            {
                if (kvs.Count > 0 || retExp != null)
                {
                    i2 = GetLiteralMatch(exp, i, ",", ";");
                    if (i2 == i)
                        break;
                    i = SkipSpace(exp, i2);
                }

                i2 = GetKvcItem(provider, nakdeMode, exp, i, out var otherItem, out var nodeOtherItem);
                if (i2 == i)
                    break;
                if (otherItem.Key == null)
                {
                    if (retExp != null)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(nodeOtherItem.Pos, nodeItems.Count,
                            "Duplicate return statement"));
                        return index;
                    }

                    retExp = otherItem.ValueExpression;
                }
                else
                    kvs.Add(otherItem);

                nodeItems.Add(nodeOtherItem);
                i = SkipSpace(exp, i2);
            } while (true);

            if (!nakdeMode)
            {
                i2 = GetLiteralMatch(exp, i, "}");
                if (i2 == i)
                {
                    syntaxErrors.Add(new SyntaxErrorData(i, 0, "'}' expected"));
                    return index;
                }

                i = SkipSpace(exp, i2);
            }

            if (nakdeMode)
            {
                if (kvs.Count == 0 && retExp == null)
                    return index;
            }

            kvcExpr = new KvcExpression();
            kvcExpr.SetContext(provider);
            var error = kvcExpr.SetKeyValues(kvs.ToArray(), retExp);
            if (error != null)
            {
                syntaxErrors.Add(new SyntaxErrorData(index, i - index, error));
                return index;
            }

            parseNode = new ParseNode(ParseNodeType.KeyValueCollection, index, i - index, nodeItems);
            return i;
        }
    }
}