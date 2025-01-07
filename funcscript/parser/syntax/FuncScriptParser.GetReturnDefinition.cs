using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetReturnDefinition(ParseContext context, int index, bool allowImplicitReturn)
        {
            ParseNode parseNode = null;
            ExpressionBlock retExp = null;

            var i = GetLiteralMatch(context, index, KW_RETURN).NextIndex;
            if (i == index)
            {
                if (allowImplicitReturn)
                    return GetExpression(context, index);
                return new ExpressionBlockResult(null, null, index);
            }

            var nodeReturn = new ParseNode(ParseNodeType.KeyWord, index, i - index);
            i = SkipSpace(context, i).NextIndex;

            var exprResult = GetExpression(context, i);
            if (exprResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "return expression expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = exprResult.NextIndex;
            retExp = exprResult.Block;
            retExp.CodePos = index;
            retExp.CodeLength = i - index;
            parseNode = new ParseNode(ParseNodeType.ReturnExpression, index, i - index,
                new[] { nodeReturn, exprResult.ParseNode });

            return new ExpressionBlockResult(retExp, parseNode, i);
        }
    }
}
