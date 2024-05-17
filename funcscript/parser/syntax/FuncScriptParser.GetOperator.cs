using funcscript.funcs.math;
using funcscript.funcs.logic;
using funcscript.model;
using funcscript.nodes;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetOperator(IFsDataProvider parseContext, string[] candidates, string exp, int index,
            out string matechedOp, out IFsFunction oper,
            out ParseNode parseNode)
        {
            foreach (var op in candidates)
            {
                var i = GetLiteralMatch(exp, index, op);
                if (i <= index) continue;

                var func = parseContext.GetData(op);
//                if (func is not IFsFunction f) 
//                    continue;

                oper = func as IFsFunction;
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
