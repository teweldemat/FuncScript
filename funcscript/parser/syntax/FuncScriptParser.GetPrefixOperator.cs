using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;
using Microsoft.VisualBasic;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetPrefixOperator(ParseContext context, int index)
        {
            int i = 0;
            (var oper, var opNode, i) = GetOperator(context, s_prefixOp.Select(x => x[0]).ToArray(), index);

            if (i == index)
            {
                return new ExpressionBlockResult(null, null, index);
            }

            var symbol = s_prefixOp.First(x => x[0] == oper)[1];
            var func = context.ReferenceProvider.Get(symbol);
            if (func == null)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(index, i - index, $"Prefix operator {oper} not defined"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = SkipSpace(context, i).NextIndex;

            var operandRes = GetCallAndMemberAccess(context, i);
            if (operandRes.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, $"Operant for {oper} expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = SkipSpace(context, operandRes.NextIndex).NextIndex;

            var expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = new[] { operandRes.Block },
                CodePos = index,
                CodeLength = i - index,
            };

            var parseNode = new ParseNode(ParseNodeType.PrefixOperatorExpression, index, i - index,
            new[] { opNode, operandRes.ParseNode });

            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}
