using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static ExpressionBlockResult GetInfixFunctionCall(ParseContext context, int index)
        {
            var childNodes = new List<ParseNode>();
            var allOperands = new List<ExpressionBlock>();

            var result = GetCallAndMemberAccess(context, index);
            if (result.NextIndex == index)
            {
                return new ExpressionBlockResult(null, null, index);
            }
            var prog = result.Block;
            var parseNode = result.ParseNode;

            allOperands.Add(prog);
            childNodes.Add(parseNode);
            var i = SkipSpace(context, result.NextIndex).NextIndex;

            var identifierResult = GetIdentifier(context, i, false);
            if (identifierResult.NextIndex == i)
            {
                return new ExpressionBlockResult(prog, parseNode, i);
            }
            var func = context.ReferenceProvider.Get(identifierResult.IdenLower);
            if (!(func is IFsFunction inf))
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, identifierResult.NextIndex - i, "A function expected"));
                return new ExpressionBlockResult(null, null, index);
            }
            if (inf.CallType != CallType.Dual)
            {
                return new ExpressionBlockResult(prog, parseNode, i);
            }

            childNodes.Add(identifierResult.ParseNode);
            i = SkipSpace(context, identifierResult.NextIndex).NextIndex;

            var secondParamResult = GetCallAndMemberAccess(context, i);
            if (secondParamResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, $"Right side operand expected for {identifierResult.Iden}"));
                return new ExpressionBlockResult(null, null, index);
            }

            allOperands.Add(secondParamResult.Block);
            childNodes.Add(secondParamResult.ParseNode);
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

                allOperands.Add(moreOperandResult.Block);
                childNodes.Add(moreOperandResult.ParseNode);
            }

            if (allOperands.Count < 2)
            {
                return new ExpressionBlockResult(null, null, index);
            }

            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(func),
                Parameters = allOperands.ToArray()
            };
            prog.SetContext(context.ReferenceProvider);
            parseNode = new ParseNode(ParseNodeType.GeneralInfixExpression, childNodes[0].Pos,
                childNodes[^1].Pos + childNodes[^1].Length + childNodes[0].Pos);

            return new ExpressionBlockResult(prog, parseNode, i);
        }
    }
}