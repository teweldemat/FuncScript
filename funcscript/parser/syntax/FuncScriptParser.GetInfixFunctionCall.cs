using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetInfixFunctionCall(KeyValueCollection provider, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var childNodes = new List<ParseNode>();
            var allOperands = new List<ExpressionBlock>();

            var i = GetCallAndMemberAccess(provider, exp, index, out var firstParam, out var firstPramNode,
                syntaxErrors);
            if (i == index)
            {
                prog = null;
                parseNode = null;
                return index;
            }
            prog = firstParam;
            parseNode = firstPramNode;

            allOperands.Add(firstParam);
            childNodes.Add(firstPramNode);
            i = SkipSpace(exp, i);

            var i2 = GetIdentifier(provider, exp, i, false, out var iden, out var idenLower, out _, out var idenNode);
            if (i2 == i)
            {
                return i;
            }
            var func = provider.Get(idenLower);
            if (!(func is IFsFunction inf))
            {
                prog = null;
                parseNode = null;
                syntaxErrors.Add(new SyntaxErrorData(i, i2 - i, "A function expected"));
                return index;
            }
            if (inf.CallType != CallType.Dual)
            {
                return i;
            }

            childNodes.Add(idenNode);
            i = SkipSpace(exp, i2);

            i2 = GetCallAndMemberAccess(provider, exp, i, out var secondParam, out var secondParamNode, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, $"Right side operand expected for {iden}"));
                prog = null;
                parseNode = null;
                return index;
            }

            allOperands.Add(secondParam);
            childNodes.Add(secondParamNode);
            i = SkipSpace(exp, i2);

            while (true)
            {
                i2 = GetLiteralMatch(exp, i, "~");
                if (i2 == i)
                    break;
                i = SkipSpace(exp, i2);
                i2 = GetCallAndMemberAccess(provider, exp, i, out var moreOperand, out var morePrseNode, syntaxErrors);
                if (i2 == i)
                    break;
                i = SkipSpace(exp, i2);

                allOperands.Add(moreOperand);
                childNodes.Add(morePrseNode);
            }

            if (allOperands.Count < 2)
            {
                prog = null;
                parseNode = null;
                return index;
            }

            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = allOperands.ToArray()
            };
            prog.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.GeneralInfixExpression, childNodes[0].Pos,
                childNodes[^1].Pos + childNodes[^1].Length + childNodes[0].Pos);

            return i;
        }
    }
}