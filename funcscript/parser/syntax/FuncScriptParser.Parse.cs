using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        // static int GetPrefixOperator(IFsDataProvider parseContext, string exp, int index, out ExpressionBlock prog,
        //     out ParseNode parseNode, List<SyntaxErrorData> serrors)
        // {
        //     int i = 0;
        //     string oper = null;
        //     foreach (var op in s_prefixOp)
        //     {
        //         i = GetLiteralMatch(exp, index, op[0]);
        //         if (i > index)
        //         {
        //             oper = op[1];
        //             break;
        //         }
        //     }
        //
        //     if (i == index)
        //     {
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     i = SkipSpace(exp, i);
        //     var func = parseContext.Get(oper);
        //     if (func == null)
        //     {
        //         serrors.Add(new SyntaxErrorData(index, i - index, $"Prefix operator {oper} not defined"));
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     var i2 = GetCallAndMemberAccess(parseContext, exp, i, out var operand, out var operandNode, serrors);
        //     if (i2 == i)
        //     {
        //         serrors.Add(new SyntaxErrorData(i, 0, $"Operant for {oper} expected"));
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     i = SkipSpace(exp, i2);
        //
        //     prog = new FunctionCallExpression
        //     {
        //         Provider = parseContext,
        //         Function = new LiteralBlock(func),
        //         Parameters = new[] { operand },
        //         CodePos = index,
        //         CodeLength = i - index,
        //     };
        //     parseNode = new ParseNode(ParseNodeType.PrefixOperatorExpression, index, i - index);
        //     return i;
        // }

        public static ExpressionBlock Parse(IFsDataProvider context, String exp, List<SyntaxErrorData> serrors)
        {
            return Parse(context, exp, out var node, serrors);
        }

        public static ExpressionBlock Parse(IFsDataProvider context, String exp, out ParseNode parseNode,
            List<SyntaxErrorData> serrors)
        {
            var i = GetRootExpression(context, exp, 0, out var prog, out parseNode, serrors);
            if (prog != null)
            {
                prog.Provider = context;
            }
            return prog;
        }
    }
}