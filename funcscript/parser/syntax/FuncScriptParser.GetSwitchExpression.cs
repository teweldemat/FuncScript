using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetSwitchExpression(KeyValueCollection provider, String exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            prog = null;
            parseNode = null;
            var i = index;
            var i2 = GetLiteralMatch(exp, i, KW_SWITCH);
            if (i2 == i)
                return index;
            i = SkipSpace(exp, i2);
            var pars = new List<ExpressionBlock>();
            var childNodes = new List<ParseNode>();
            i2 = GetExpression(provider, exp, i, out var partSelector, out var nodeSelector, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 1, "Switch selector expected"));
                return index;
            }

            pars.Add(partSelector);
            childNodes.Add(nodeSelector);
            i = SkipSpace(exp, i2);
            do
            {
                i2 = GetLiteralMatch(exp, i, ",", ";");
                if (i2 == i)
                    break;
                i = SkipSpace(exp, i2);
                i2 = GetExpression(provider, exp, i, out var part1, out var part1Node, syntaxErrors);
                if (i2 == i)
                {
                    break;
                }

                i = SkipSpace(exp, i2);
                pars.Add(part1);
                childNodes.Add(part1Node);

                i2 = GetLiteralMatch(exp, i, ":");
                if (i2 == i)
                {
                    break;
                }

                i = SkipSpace(exp, i2);
                i2 = GetExpression(provider, exp, i, out var part2, out var part2Node, syntaxErrors);
                if (i2 == i)
                {
                    syntaxErrors.Add(new SyntaxErrorData(i, 1, "Selector result expected"));
                    return index;
                }

                pars.Add(part2);
                childNodes.Add(part2Node);
                i = SkipSpace(exp, i2);
            } while (true);

            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(provider.Get(KW_SWITCH)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray(),
            };
            prog.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.Case, index, index - i);
            parseNode.Childs = childNodes;
            return i;
        }
    }
}