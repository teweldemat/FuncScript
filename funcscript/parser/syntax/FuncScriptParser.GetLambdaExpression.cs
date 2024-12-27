using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetLambdaExpression(KeyValueCollection provider, string exp, int index, out ExpressionFunction func,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            func = null;

            var i = GetIdentifierList(provider, exp, index, out var parms, out var nodesParams);
            if (i == index)
                return index;

            i = SkipSpace(exp, i);
            if (i >= exp.Length - 1) // we need two characters
                return index;
            var i2 = GetLiteralMatch(exp, i, "=>");
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "'=>' expected"));
                return index;
            }

            i += 2;
            i = SkipSpace(exp, i);
            var parmsSet = new HashSet<string>();
            foreach (var p in parms)
            {
                parmsSet.Add(p);
            }

            i2 = GetExpression(provider, exp, i, out var defination, out var nodeDefination, syntaxErrors);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "defination of lambda expression expected"));
                return index;
            }

            func = new ExpressionFunction(parms.ToArray(), defination);
            func.SetContext(provider);
            i = i2;
            parseNode = new ParseNode(ParseNodeType.LambdaExpression, index, i - index,
                new[] { nodesParams, nodeDefination });
            return i;
        }
    }
}