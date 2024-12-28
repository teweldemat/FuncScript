using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetOperatorResult(string MatchedOp, IFsFunction Oper, ParseNode ParseNode, int NextIndex);

        static GetOperatorResult GetOperator(ParseContext context, string[] candidates, int index)
        {
            foreach (var op in candidates)
            {
                var literalMatchResult = GetLiteralMatch(context, index, op);
                var i = literalMatchResult.NextIndex;
                if (i <= index) continue;

                var func = context.Provider.Get(op);
                var oper = func as IFsFunction;
                if (oper != null && oper is ExpressionBlock expressionBlock)
                {
                    expressionBlock.SetContext(context.Provider);
                }

                var parseNode = new ParseNode(ParseNodeType.Operator, index, i - index);
                return new GetOperatorResult(op, oper, parseNode, i);
            }

            return new GetOperatorResult(null, null, null, index);
        }
    }
}