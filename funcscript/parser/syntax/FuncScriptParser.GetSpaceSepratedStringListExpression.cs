using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetSpaceSepratedStringListExpression(KeyValueCollection provider, string exp, int index,
            out List<string> stringList, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            stringList = null;
            var i = SkipSpace(exp, index);

            var listItems = new List<string>();
            var nodeListItems = new List<ParseNode>();
            string firstItem;
            ParseNode firstNode;

            string otherItem;
            ParseNode otherNode;
            var i2 = GetSimpleString(provider, exp, i, out firstItem, out firstNode, syntaxErrors);
            if (i2 == i)
                i2 = GetSpaceLessString(provider, exp, i, out firstItem, out firstNode);
            if (i2 > i)
            {
                listItems.Add(firstItem);
                nodeListItems.Add(firstNode);
                i = i2;
                do
                {
                    i2 = GetLiteralMatch(exp, i, " ");
                    if (i2 == i)
                        break;
                    i = i2;
                    i = SkipSpace(exp, i);
                    i2 = GetSimpleString(provider, exp, i, out otherItem, out otherNode, syntaxErrors);
                    if (i2 == i)
                        i2 = GetSpaceLessString(provider, exp, i, out otherItem, out otherNode);

                    if (i2 == i)
                        break;
                    listItems.Add(otherItem);
                    nodeListItems.Add(otherNode);
                    i = i2;
                } while (true);
            }

            stringList = listItems;
            parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return i;
        }
    }
}