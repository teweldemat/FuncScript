using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetInfixFunctionCallResult(ExpressionBlock Program, ParseNode ParseNode, int NextIndex);

        static GetInfixFunctionCallResult GetInfixFunctionCall(ParseContext context, int index)
        {
            var childNodes = new List<ParseNode>();
            var allOperands = new List<ExpressionBlock>();

            var result = GetCallAndMemberAccess(context, index);
            if (result.NextIndex == index)
            {
                return new GetInfixFunctionCallResult(null, null, index);
            }
            var prog = result.Expression;
            var parseNode = result.Node;

            allOperands.Add(prog);
            childNodes.Add(parseNode);
            var i = SkipSpace(context, result.NextIndex).NextIndex;

            var identifierResult = GetIdentifier(context, i, false);
            if (identifierResult.NextIndex == i)
            {
                return new GetInfixFunctionCallResult(prog, parseNode, i);
            }
            var func = context.Provider.Get(identifierResult.IdenLower);
            if (!(func is IFsFunction inf))
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, identifierResult.NextIndex - i, "A function expected"));
                return new GetInfixFunctionCallResult(null, null, index);
            }
            if (inf.CallType != CallType.Dual)
            {
                return new GetInfixFunctionCallResult(prog, parseNode, i);
            }

            childNodes.Add(identifierResult.ParseNode);
            i = SkipSpace(context, identifierResult.NextIndex).NextIndex;

            var secondParamResult = GetCallAndMemberAccess(context, i);
            if (secondParamResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, $"Right side operand expected for {identifierResult.Iden}"));
                return new GetInfixFunctionCallResult(null, null, index);
            }

            allOperands.Add(secondParamResult.Expression);
            childNodes.Add(secondParamResult.Node);
            i = SkipSpace(context, secondParamResult.NextIndex).NextIndex;

            while (true)
            {
                var literalMatchResult = GetLiteralMatch(context, i, new string[] { "~" });
                if (literalMatchResult.NextIndex == i)
                    break;
                i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;
                var moreOperandResult = GetCallAndMemberAccess(context, i);
                if (moreOperandResult.NextIndex == i)
                    break;
                i = SkipSpace(context, moreOperandResult.NextIndex).NextIndex;

                allOperands.Add(moreOperandResult.Expression);
                childNodes.Add(moreOperandResult.Node);
            }

            if (allOperands.Count < 2)
            {
                return new GetInfixFunctionCallResult(null, null, index);
            }

            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = allOperands.ToArray()
            };
            prog.SetContext(context.Provider);
            parseNode = new ParseNode(ParseNodeType.GeneralInfixExpression, childNodes[0].Pos,
                childNodes[^1].Pos + childNodes[^1].Length + childNodes[0].Pos);

            return new GetInfixFunctionCallResult(prog, parseNode, i);
        }
    }
}