using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetMemberAccessResult(ExpressionBlock Expression, ParseNode Node, int NextIndex);

        static GetMemberAccessResult GetMemberAccess(ParseContext context, ExpressionBlock source, int index)
        {
            var result = GetMemberAccessInternal(context, ".", source, index);
            if (result.NextIndex == index)
                return GetMemberAccessInternal(context, "?.", source, index);
            return result;
        }

        static GetMemberAccessResult GetMemberAccessInternal(ParseContext context, string oper, ExpressionBlock source, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock prog = null;
            var i = SkipSpace(context, index).NextIndex;
            var i2 = GetLiteralMatch(context, i, oper).NextIndex;
            if (i2 == i)
                return new GetMemberAccessResult(null, null, index);

            i = i2;
            i = SkipSpace(context, i).NextIndex;
            var identifierResult = GetIdentifier(context, i, false);

            if (identifierResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "member identifier expected"));
                return new GetMemberAccessResult(null, null, index);
            }

            i = identifierResult.NextIndex;
            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(context.Provider.Get(oper)),
                Parameters = new ExpressionBlock[] { source, new LiteralBlock(identifierResult.Iden) },
                CodePos = source.CodePos,
                CodeLength = i - source.CodePos
            };
            prog.SetContext(context.Provider);
            return new GetMemberAccessResult(prog, parseNode, i);
        }
    }
}