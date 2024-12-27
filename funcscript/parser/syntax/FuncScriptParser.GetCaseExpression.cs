using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetCaseExpression(KeyValueCollection provider, string exp, int index, out ExpressionBlock expBlock,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            expBlock = null;
            parseNode = null;
            var i = index;
            var i2 = GetLiteralMatch(exp, i, KW_CASE);
            if (i2 == i)
                return index;
            i = SkipSpace(exp, i2);
            var pars = new List<ExpressionBlock>();
            var childNodes = new List<ParseNode>();
            do
            {
                if (pars.Count == 0)
                {
                    i2 = GetExpression(provider, exp, i, out var part1, out var part1Node, syntaxErrors);
                    if (i2 == i)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(i, 1, "Case condition expected"));
                        return index;
                    }

                    pars.Add(part1);
                    childNodes.Add(part1Node);
                    i = SkipSpace(exp, i2);
                }
                else
                {
                    i2 = GetLiteralMatch(exp, i, ",", ";");
                    if (i2 == i)
                        break;
                    i = SkipSpace(exp, i2);
                    i2 = GetExpression(provider, exp, i, out var part1, out var part1Node, syntaxErrors);
                    if (i2 == i)
                        break;
                    pars.Add(part1);
                    childNodes.Add(part1Node);
                    i = SkipSpace(exp, i2);
                }

                i2 = GetLiteralMatch(exp, i, ":");
                if (i2 == i)
                {
                    break;
                }

                i = SkipSpace(exp, i2);
                i2 = GetExpression(provider, exp, i, out var part2, out var part2Node, syntaxErrors);
                if (i2 == i)
                {
                    syntaxErrors.Add(new SyntaxErrorData(i, 1, "Case value expected"));
                    return index;
                }

                pars.Add(part2);
                childNodes.Add(part2Node);
                i = SkipSpace(exp, i2);
            } while (true);

            expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(provider.Get(KW_CASE)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray(),
            };
            expBlock.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.Case, index, index - i);
            parseNode.Childs = childNodes;
            return i;
        }
    }
}