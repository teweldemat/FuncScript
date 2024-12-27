using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetSpaceSepratedListExpression(KeyValueCollection provider, string exp, int index,
            out ListExpression listExpr, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            listExpr = null;
            var i = SkipSpace(exp, index);

            var listItems = new List<ExpressionBlock>();
            var nodeListItems = new List<ParseNode>();
            var i2 = GetExpression(provider, exp, i, out var firstItem, out var nodeFirstItem, syntaxErrors);
            if (i2 > i)
            {
                listItems.Add(firstItem);
                nodeListItems.Add(nodeFirstItem);
                i = i2;
                do
                {
                    i2 = GetLiteralMatch(exp, i, " ");
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

            listExpr = new ListExpression { ValueExpressions = listItems.ToArray() };
            listExpr.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return i;
        }
    }
}