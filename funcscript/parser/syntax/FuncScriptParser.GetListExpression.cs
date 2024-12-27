using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetListExpression(KeyValueCollection provider, String exp, int index, out ListExpression listExpr,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            listExpr = null;
            var i = SkipSpace(exp, index);
            var i2 = GetLiteralMatch(exp, i, "[");
            if (i2 == i)
                return index; //we didn't find '['
            i = i2;

            var listItems = new List<ExpressionBlock>();
            var nodeListItems = new List<ParseNode>();
            i = SkipSpace(exp, i);
            i2 = GetExpression(provider, exp, i, out var firstItem, out var nodeFirstItem, syntaxErrors);
            if (i2 > i)
            {
                listItems.Add(firstItem);
                nodeListItems.Add(nodeFirstItem);
                i = i2;
                do
                {
                    i = SkipSpace(exp, i);
                    i2 = GetLiteralMatch(exp, i, ",");
                    if (i2 == i)
                        break;
                    i = i2;

                    i = SkipSpace(exp, i);
                    i2 = GetExpression(provider, exp, i, out var otherItem, out var nodeOtherItem, syntaxErrors);
                    if (i2 == i)
                        break;
                    listItems.Add(otherItem);
                    nodeListItems.Add(nodeOtherItem);
                    i = i2;
                } while (true);
            }

            i = SkipSpace(exp, i);
            i2 = GetLiteralMatch(exp, i, "]");
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "']' expected"));
                return index;
            }

            i = i2;
            listExpr = new ListExpression
            {
                ValueExpressions = listItems.ToArray()
            };
            listExpr.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return i;
        }
    }
}