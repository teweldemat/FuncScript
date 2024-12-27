using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetPrefixOperator(KeyValueCollection provider, string exp, int index, out ExpressionBlock expBlock,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            int i = 0;
            string oper = null;
            foreach (var op in s_prefixOp)
            {
                i = GetLiteralMatch(exp, index, op[0]);
                if (i > index)
                {
                    oper = op[1];
                    break;
                }
            }

            if (i == index)
            {
                expBlock = null;
                parseNode = null;
                return index;
            }

            i = SkipSpace(exp, i);
            var func = provider.Get(oper);
            if (func == null)
            {
                syntaxErrors.Add(new SyntaxErrorData(index, i - index, $"Prefix operator {oper} not defined"));
                expBlock = null;
                parseNode = null;
                return index;
            }

            var i2 = GetCallAndMemberAccess(provider, exp, i, out var operand, out var operandNode, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, $"Operant for {oper} expected"));
                expBlock = null;
                parseNode = null;
                return index;
            }

            i = SkipSpace(exp, i2);

            expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = new[] { operand },
                CodePos = index,
                CodeLength = i - index,
            };
            expBlock.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.PrefixOperatorExpression, index, i - index);
            return i;
        }
    }
}