using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetMemberAccess(KeyValueCollection provider, ExpressionBlock source, String exp, int index,
            out ExpressionBlock prog, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var i2 = GetMemberAccess(provider, ".", source, exp, index, out prog, out parseNode, syntaxErrors);
            if (i2 == index)
                return GetMemberAccess(provider, "?.", source, exp, index, out prog, out parseNode, syntaxErrors);
            return i2;
        }

        static int GetMemberAccess(KeyValueCollection provider, string oper, ExpressionBlock source, String exp, int index,
            out ExpressionBlock prog, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            prog = null;
            var i = SkipSpace(exp, index);
            var i2 = GetLiteralMatch(exp, i, oper);
            if (i2 == i)
                return index;
            i = i2;
            i = SkipSpace(exp, i);
            i2 = GetIdentifier(provider, exp, i, false, out var member, out var memberLower, out _, out parseNode);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "member identifier expected"));
                return index;
            }

            i = i2;
            prog = new FunctionCallExpression
            {
                Function = new LiteralBlock(provider.Get(oper)),
                Parameters = new ExpressionBlock[] { source, new LiteralBlock(member) },
                CodePos = source.CodePos,
                CodeLength = i - source.CodePos
            };
            prog.SetContext(provider);
            return i;
        }
    }
}