using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetOperator(KeyValueCollection parseContext, string[] candidates, string exp, int index,
            out string matechedOp, out IFsFunction oper,
            out ParseNode parseNode)
        {
            foreach (var op in candidates)
            {
                var i = GetLiteralMatch(exp, index, op);
                if (i <= index) continue;

                var func = parseContext.Get(op);
                oper = func as IFsFunction;
                if (oper != null && oper is ExpressionBlock expressionBlock)
                {
                    expressionBlock.SetContext(parseContext);
                }

                parseNode = new ParseNode(ParseNodeType.Operator, index, i - index);
                matechedOp = op;
                return i;
            }

            oper = null;
            parseNode = null;
            matechedOp = null;
            return index;
        }
    }
}