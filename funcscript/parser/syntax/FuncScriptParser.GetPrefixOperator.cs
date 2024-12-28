using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetPrefixOperator(ParseContext context, int index)
        {
            int i = 0;
            string oper = null;
            foreach (var op in s_prefixOp)
            {
                i = GetLiteralMatch(context, index, op[0]).NextIndex;
                if (i > index)
                {
                    oper = op[1];
                    break;
                }
            }

            if (i == index)
            {
                return new ExpressionBlockResult(null, null, index);
            }

            i = SkipSpace(context, i).NextIndex;
            var func = context.Provider.Get(oper);
            if (func == null)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(index, i - index, $"Prefix operator {oper} not defined"));
                return new ExpressionBlockResult(null, null, index);
            }

            var callResult = GetCallAndMemberAccess(context, i);
            if (callResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, $"Operant for {oper} expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = SkipSpace(context, callResult.NextIndex).NextIndex;

            var expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = new[] { callResult.Expression },
                CodePos = index,
                CodeLength = i - index,
            };
            expBlock.SetContext(context.Provider);
            var parseNode = new ParseNode(ParseNodeType.PrefixOperatorExpression, index, i - index);
            
            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}