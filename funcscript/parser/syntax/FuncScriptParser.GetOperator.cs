using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record GetOperatorResult(string MatchedOp, ParseNode ParseNode, int NextIndex)
            : ParseResult(ParseNode, NextIndex);

        static GetOperatorResult GetOperator(ParseContext context, string[] candidates, int index)
        {
            foreach (var op in candidates)
            {
                var literalMatchResult = GetLiteralMatch(context, index, op);
                var i = literalMatchResult.NextIndex;
                if (i <= index) continue;

                var parseNode = new ParseNode(ParseNodeType.Operator, index, i - index);
                return new GetOperatorResult(op, parseNode, i);
            }

            return new GetOperatorResult(null, null, index);
        }
    }
}
