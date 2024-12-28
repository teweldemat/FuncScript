using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetSwitchExpressionResult(ExpressionBlock Prog, ParseNode ParseNode, int NextIndex);

        static GetSwitchExpressionResult GetSwitchExpression(ParseContext context, int index)
        {
            ExpressionBlock prog = null;
            ParseNode parseNode = null;
            var i = index;
            var i2 = GetLiteralMatch(context, i, KW_SWITCH).NextIndex;
            if (i2 == i)
                return new GetSwitchExpressionResult(null, null, index);

            i = SkipSpace(context, i2).NextIndex;
            var pars = new List<ExpressionBlock>();
            var childNodes = new List<ParseNode>();
            
            var expressionResult = GetExpression(context, i);
            if (expressionResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Switch selector expected"));
                return new GetSwitchExpressionResult(null, null, index);
            }

            pars.Add(expressionResult.Expression);
            childNodes.Add(expressionResult.Node);
            i = SkipSpace(context, expressionResult.NextIndex).NextIndex;
            
            while (true)
            {
                i2 = GetLiteralMatch(context, i, ",", ";").NextIndex;
                if (i2 == i)
                    break;
                i = SkipSpace(context, i2).NextIndex;

                var part1Result = GetExpression(context, i);
                if (part1Result.NextIndex == i)
                    break;

                i = SkipSpace(context, part1Result.NextIndex).NextIndex;
                pars.Add(part1Result.Expression);
                childNodes.Add(part1Result.Node);

                i2 = GetLiteralMatch(context, i, ":").NextIndex;
                if (i2 == i)
                    break;

                i = SkipSpace(context, i2).NextIndex;

                var part2Result = GetExpression(context, i);
                if (part2Result.NextIndex == i)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Selector result expected"));
                    return new GetSwitchExpressionResult(null, null, index);
                }

                pars.Add(part2Result.Expression);
                childNodes.Add(part2Result.Node);
                i = SkipSpace(context, part2Result.NextIndex).NextIndex;
            }

            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(context.Provider.Get(KW_SWITCH)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray(),
            };
            prog.SetContext(context.Provider);
            parseNode = new ParseNode(ParseNodeType.Case, index, i - index);
            parseNode.Childs = childNodes;
            return new GetSwitchExpressionResult(prog, parseNode, i);
        }
    }
}