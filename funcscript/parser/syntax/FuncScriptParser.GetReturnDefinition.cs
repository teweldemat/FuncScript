using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetReturnDefinition(KeyValueCollection provider, String exp, int index, out ExpressionBlock retExp,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            retExp = null;
            var i = GetLiteralMatch(exp, index, KW_RETURN);
            if (i == index)
                return index;
            var nodeReturn = new ParseNode(ParseNodeType.KeyWord, index, i - index);
            i = SkipSpace(exp, i);
            var i2 = GetExpression(provider, exp, i, out var expBlock, out var nodeExpBlock, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "return expression expected"));
                return index;
            }

            i = i2;
            retExp = expBlock;
            retExp.SetContext(provider);
            retExp.CodePos = index;
            retExp.CodeLength = i - index;
            parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index,
                new[] { nodeReturn, nodeExpBlock });

            return i;
        }
    }
}