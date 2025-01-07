using System.Text;
using FuncScript.Block;
using FuncScript.Model;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult GetStringTemplate(ParseContext context, int index)
        {
            var result = GetStringTemplate(context, "\"", index);
            if (result.NextIndex > index)
                return result;
            return GetStringTemplate(context, "'", index);
        }

        public static ExpressionBlockResult GetStringTemplate(ParseContext context, string delimiter, int index)
        {
            var provider = context.ReferenceProvider;
            var exp = context.Expression;
            var syntaxErrors = context.SyntaxErrors;
            ExpressionBlock prog = null;
            ParseNode parseNode = null;
            var parts = new List<ExpressionBlock>();
            var nodeParts = new List<ParseNode>();

            var i = GetLiteralMatch(context, index, $"f{delimiter}").NextIndex;
            if (i == index)
                return new ExpressionBlockResult(prog, parseNode, index);
            
            var lastIndex = i;
            var sb = new StringBuilder();
            int i2;
            while (true)
            {
                i2 = GetLiteralMatch(context, i, @"\\").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\\');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\n").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\n');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\t").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\t');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, $@"\{delimiter}").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append(delimiter);
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\{").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append("{");
                    continue;
                }

                i2 = GetLiteralMatch(context, i, "{").NextIndex;
                if (i2 > i)
                {
                    if (sb.Length > 0)
                    {
                        var lb = new LiteralBlock(sb.ToString());
                        parts.Add(lb);
                        nodeParts.Add(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
                        sb = new StringBuilder();
                    }

                    i = i2;
                    i = SkipSpace(context, i).NextIndex;
                    var exprResult = GetExpression(new ParseContext(provider, exp, syntaxErrors), i);
                    if (exprResult.NextIndex == i)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(i, 0, "expression expected"));
                        return new ExpressionBlockResult(prog, parseNode, index);
                    }

                    parts.Add(exprResult.Block);
                    nodeParts.Add(exprResult.ParseNode);
                    i = exprResult.NextIndex;
                    i2 = GetLiteralMatch(context, i, "}").NextIndex;
                    if (i2 == i)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(i, 0, "'}' expected"));
                        return new ExpressionBlockResult(prog, parseNode, index);
                    }

                    i = i2;
                    lastIndex = i;
                    continue;
                }

                if (i >= exp.Length || GetLiteralMatch(context, i, delimiter).NextIndex > i)
                    break;

                sb.Append(exp[i]);
                i++;
            }

            if (i > lastIndex)
            {
                if (sb.Length > 0)
                {
                    var lb = new LiteralBlock(sb.ToString());
                    parts.Add(lb);
                    nodeParts.Add(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
                }
            }

            i2 = GetLiteralMatch(context, i, delimiter).NextIndex;
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, $"'{delimiter}' expected"));
                return new ExpressionBlockResult(prog, parseNode, index);
            }

            i = i2;

            if (parts.Count == 0)
            {
                prog = new LiteralBlock("");
                parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
            }
            else if (parts.Count == 1)
            {
                prog = parts[0];
                parseNode = nodeParts[0];
            }
            else
            {
                prog = new FunctionCallExpression
                {
                    Function = new LiteralBlock(provider.Get("+")),
                    Parameters = parts.ToArray()
                };
                parseNode = new ParseNode(ParseNodeType.StringTemplate, index, i - index, nodeParts);
            }

            return new ExpressionBlockResult(prog, parseNode, i);
        }

    }
}
